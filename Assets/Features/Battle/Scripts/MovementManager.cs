using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Features.Tiles;
using Features.Unit.Battle.Scripts;
using Features.Unit.Modding;
using Photon.Pun;
using Photon.Realtime;
using ThirdParty.LeanTween.Framework;
using UnityEngine;

namespace Features.Battle.Scripts
{
    /// <summary>
    /// Make sure every 'MovementManager' exists only once for each Battle -> marked by 'BattleData_SO'
    /// </summary>
    public class MovementManager : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        [Header("References")]
        [SerializeField] private BattleData_SO battleData;

        public override void OnEnable()
        {
            base.OnEnable();

            MovementState.onPerformMovement += RequestMove;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            
            MovementState.onPerformMovement -= RequestMove;
        }
        
        private bool RequestMove(BattleBehaviour battleBehaviour, Vector3Int targetTileGridPosition, int skipLastMovementsCount)
        {
            Vector3Int currentCellPosition = CurrentCellPosition(battleBehaviour);
            if (PhotonNetwork.IsMasterClient)
            {
                OnMasterChangeUnitGridPosition(battleBehaviour, targetTileGridPosition, currentCellPosition, skipLastMovementsCount, battleBehaviour.MovementSpeed);
            }
            else
            {
                RequestMoveToTarget(battleBehaviour.NetworkedStatsBehaviour.PhotonView, targetTileGridPosition, currentCellPosition, skipLastMovementsCount, battleBehaviour.MovementSpeed);
            }

            return true;
        }
    
        private Vector3Int CurrentCellPosition(BattleBehaviour battleBehaviour)
        {
            return battleData.TileRuntimeDictionary.GetWorldToCellPosition(battleBehaviour.transform.position);
        }

        private void OnMasterChangeUnitGridPosition(NetworkedBattleBehaviour battleBehaviour, Vector3Int targetCellPosition, Vector3Int currentCellPosition, int skipLastMovementsCount, float movementSpeed)
        {
            if (!TryGetNextPosition(targetCellPosition, currentCellPosition, skipLastMovementsCount, out Vector3Int nextCellPosition)) return;

            UpdateUnitOnRuntimeTiles(battleBehaviour, currentCellPosition, nextCellPosition);

            object[] data = new object[]
            {
                battleBehaviour.NetworkedStatsBehaviour.PhotonView.ViewID,
                nextCellPosition,
                movementSpeed
            };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.All,
                CachingOption = EventCaching.AddToRoomCache
            };

            SendOptions sendOptions = new SendOptions
            {
                Reliability = true
            };

            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnPerformGridPositionSwap, data, raiseEventOptions, sendOptions);
        }
    
        private bool TryGetNextPosition(Vector3Int targetCellPosition, Vector3Int startCellPosition, int skipLastMovementsCount, out Vector3Int nextCellPosition)
        {
            if (battleData.TileRuntimeDictionary.GenerateAStarPath(startCellPosition,
                targetCellPosition, out List<Vector3Int> path) && path.Count > skipLastMovementsCount)
            {
                nextCellPosition = path[0];
                return true;
            }

            nextCellPosition = default;
            return false;
        }
    
        private void UpdateUnitOnRuntimeTiles(NetworkedBattleBehaviour battleBehaviour, Vector3Int currentCellPosition, Vector3Int nextCellPosition)
        {
            RuntimeTile targetTileContainer = battleData.TileRuntimeDictionary.GetContent(nextCellPosition);
            targetTileContainer.AddUnit(battleBehaviour.gameObject);

            if (battleData.TileRuntimeDictionary.TryGetContent(currentCellPosition, out RuntimeTile previousTileBehaviour))
            {
                previousTileBehaviour.RemoveUnit();
            }
        }
    
        private static void RequestMoveToTarget(PhotonView photonView, Vector3Int targetCellPosition, Vector3Int currentCellPosition, int skipLastMovementsCount, float movementSpeed)
        {
            object[] data = new object[]
            {
                photonView.ViewID,
                targetCellPosition,
                currentCellPosition,
                skipLastMovementsCount,
                movementSpeed
            };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.MasterClient,
                CachingOption = EventCaching.AddToRoomCache
            };

            SendOptions sendOptions = new SendOptions
            {
                Reliability = true
            };

            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnRequestMoveToTarget, data, raiseEventOptions, sendOptions);
        }

        public void OnEvent(EventData photonEvent)
        {
            //moves unit to target position for all players
            if (photonEvent.Code == (int)RaiseEventCode.OnPerformGridPositionSwap)
            {
                object[] data = (object[]) photonEvent.CustomData;
                int viewID = (int) data[0];
                //TODO: getComponent
                if (battleData.AllUnitsRuntimeSet.TryGetUnitByViewID(viewID, out NetworkedStatsBehaviour networkedUnitBehaviour)
                    && networkedUnitBehaviour.TryGetComponent(out NetworkedBattleBehaviour battleBehaviour))
                {
                    Vector3Int nextCellPosition = (Vector3Int) data[1];
                    float movementSpeed = (float) data[2];
                    MoveGameObjectToTarget(battleBehaviour, nextCellPosition, movementSpeed, () =>
                    {
                        if (!battleBehaviour.TryRequestAttackState() || !battleBehaviour.TryRequestMovementStateByClosestUnit() || battleBehaviour.CurrentState is not DeathState)
                        {
                            battleBehaviour.ForceIdleState();
                        }
                    });
                }
            }
        
            //master calculated path
            if (photonEvent.Code == (int)RaiseEventCode.OnRequestMoveToTarget)
            {
                object[] data = (object[]) photonEvent.CustomData;
                int viewID = (int) data[0];
                //TODO: getComponent
                if (battleData.AllUnitsRuntimeSet.TryGetUnitByViewID(viewID, out NetworkedStatsBehaviour networkedUnitBehaviour)
                    && networkedUnitBehaviour.TryGetComponent(out NetworkedBattleBehaviour battleBehaviour))
                {
                    Vector3Int targetCellPosition = (Vector3Int) data[1];
                    Vector3Int currentCellPosition = (Vector3Int) data[2];
                    int skipLastMovementsCount = (int) data[3];
                    float movementSpeed = (float) data[4];
                    OnMasterChangeUnitGridPosition(battleBehaviour, targetCellPosition, currentCellPosition,
                        skipLastMovementsCount, movementSpeed);
                }
            }
        }
    
        private void MoveGameObjectToTarget(NetworkedBattleBehaviour battleBehaviour, Vector3Int nextCellPosition, float movementSpeed, Action onComplete)
        {
            if (movementSpeed <= 0)
            {
                Debug.LogError("The Movement Speed is to low and thus no movement is applied!");
                onComplete.Invoke();
                return;
            }
            
            Vector3 targetPosition = battleData.TileRuntimeDictionary.GetCellToWorldPosition(nextCellPosition);
            float time = Vector3.Distance(battleBehaviour.transform.position, targetPosition) / movementSpeed;
            LeanTween.move(battleBehaviour.gameObject, targetPosition, time).setOnComplete(onComplete.Invoke);
        }
    }
}
