using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Features.GlobalReferences.Scripts;
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
        [SerializeField] private NetworkedUnitRuntimeSet_SO allUnitsRuntimeSet;
        [SerializeField] private TileRuntimeDictionary_SO tileRuntimeDictionary;
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
                OnMasterChangeUnitGridPosition(battleBehaviour, targetTileGridPosition, currentCellPosition, skipLastMovementsCount);
            }
            else
            {
                RequestMoveToTarget(battleBehaviour.NetworkedUnitBehaviour.PhotonView, targetTileGridPosition, currentCellPosition, skipLastMovementsCount);
            }

            return true;
        }
    
        private Vector3Int CurrentCellPosition(BattleBehaviour battleBehaviour)
        {
            return battleData.TileRuntimeDictionary.GetWorldToCellPosition(battleBehaviour.transform.position);
        }

        private void OnMasterChangeUnitGridPosition(BattleBehaviour battleBehaviour, Vector3Int targetCellPosition, Vector3Int currentCellPosition, int skipLastMovementsCount)
        {
            if (!TryGetNextPosition(targetCellPosition, currentCellPosition, skipLastMovementsCount, out Vector3Int nextCellPosition)) return;

            UpdateUnitOnRuntimeTiles(battleBehaviour, currentCellPosition, nextCellPosition);

            object[] data = new object[]
            {
                battleBehaviour.NetworkedUnitBehaviour.PhotonView.ViewID,
                nextCellPosition
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
    
        private void UpdateUnitOnRuntimeTiles(BattleBehaviour battleBehaviour, Vector3Int currentCellPosition, Vector3Int nextCellPosition)
        {
            RuntimeTile targetTileContainer = battleData.TileRuntimeDictionary.GetContent(nextCellPosition);
            targetTileContainer.AddUnit(battleBehaviour.gameObject);

            if (battleData.TileRuntimeDictionary.TryGetContent(currentCellPosition, out RuntimeTile previousTileBehaviour))
            {
                previousTileBehaviour.RemoveUnit();
            }
        }
    
        private static void RequestMoveToTarget(PhotonView photonView, Vector3Int targetCellPosition, Vector3Int currentCellPosition, int skipLastMovementsCount)
        {
            object[] data = new object[]
            {
                photonView.ViewID,
                targetCellPosition,
                currentCellPosition,
                skipLastMovementsCount
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
                if (allUnitsRuntimeSet.TryGetUnitByViewID(viewID, out NetworkedUnitBehaviour networkedUnitBehaviour)
                    && networkedUnitBehaviour.TryGetComponent(out BattleBehaviour battleBehaviour))
                {
                    Vector3Int nextCellPosition = (Vector3Int) data[1];
                    MoveGameObjectToTarget(battleBehaviour, nextCellPosition, () =>
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
                if (allUnitsRuntimeSet.TryGetUnitByViewID(viewID, out NetworkedUnitBehaviour networkedUnitBehaviour)
                    && networkedUnitBehaviour.TryGetComponent(out BattleBehaviour battleBehaviour))
                {
                    Vector3Int targetCellPosition = (Vector3Int) data[1];
                    Vector3Int currentCellPosition = (Vector3Int) data[2];
                    int skipLastMovementsCount = (int) data[3];
                    OnMasterChangeUnitGridPosition(battleBehaviour, targetCellPosition, currentCellPosition,
                        skipLastMovementsCount);
                }
            }
        }
    
        private void MoveGameObjectToTarget(BattleBehaviour battleBehaviour, Vector3Int nextCellPosition, Action onComplete)
        {
            if (battleBehaviour.MovementSpeed <= 0)
            {
                Debug.LogError("The Movement Speed is to low and thus no movement is applied!");
                onComplete.Invoke();
                return;
            }
            
            Vector3 targetPosition = tileRuntimeDictionary.GetCellToWorldPosition(nextCellPosition);
            float time = Vector3.Distance(battleBehaviour.transform.position, targetPosition) / battleBehaviour.MovementSpeed;
            LeanTween.move(battleBehaviour.gameObject, targetPosition, time).setOnComplete(onComplete.Invoke);
        }
    }
}
