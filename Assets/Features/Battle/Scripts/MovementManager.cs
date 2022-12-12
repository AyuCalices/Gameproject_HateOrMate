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
    /// Must be a singleton, because movement events must only get called once. Else multiple movement Events will write over each other, which results in weird movement behaviour.
    /// </summary>
    public class MovementManager : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        [SerializeField] private NetworkedUnitRuntimeSet_SO allUnitsRuntimeSet;
        [SerializeField] private TileRuntimeDictionary_SO tileRuntimeDictionary;
        
        //TODO: put this as a balancing value into the unit
        [SerializeField] private float movementSpeed;

        private static TileRuntimeDictionary_SO _tileRuntimeDictionary;

        private void Awake()
        {
            _tileRuntimeDictionary = tileRuntimeDictionary;
        }

        //TODO: since this should only be called once - a singleton might be the right decision
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
                    MoveGameObjectToTarget(battleBehaviour.gameObject, nextCellPosition, () =>
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
    
        private void MoveGameObjectToTarget(GameObject movable, Vector3Int nextCellPosition, Action onComplete)
        {
            Vector3 targetPosition = tileRuntimeDictionary.GetCellToWorldPosition(nextCellPosition);
            float time = Vector3.Distance(movable.transform.position, targetPosition) / movementSpeed;
            LeanTween.move(movable, targetPosition, time).setOnComplete(onComplete.Invoke);
        }
    
        public static void RequestMove(BattleBehaviour battleBehaviour, Vector3Int targetTileGridPosition, int skipLastMovementsCount)
        {
            if (_tileRuntimeDictionary == null)
            {
                //TODO: this script should be instantiated, once it is needed ?
                Debug.LogError("This component must be added to a GameObject!");
                return;
            }
            
            Vector3Int currentCellPosition = CurrentCellPosition(battleBehaviour);
            if (PhotonNetwork.IsMasterClient)
            {
                OnMasterChangeUnitGridPosition(battleBehaviour, targetTileGridPosition, currentCellPosition, skipLastMovementsCount);
            }
            else
            {
                RequestMoveToTarget(battleBehaviour.NetworkedUnitBehaviour.PhotonView, targetTileGridPosition, currentCellPosition, skipLastMovementsCount);
            }
        }
    
        private static Vector3Int CurrentCellPosition(BattleBehaviour battleBehaviour)
        {
            return _tileRuntimeDictionary.GetWorldToCellPosition(battleBehaviour.transform.position);
        }

        private static void OnMasterChangeUnitGridPosition(BattleBehaviour battleBehaviour, Vector3Int targetCellPosition, Vector3Int currentCellPosition, int skipLastMovementsCount)
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
    
        private static bool TryGetNextPosition(Vector3Int targetCellPosition, Vector3Int startCellPosition, int skipLastMovementsCount, out Vector3Int nextCellPosition)
        {
            if (_tileRuntimeDictionary.GenerateAStarPath(startCellPosition,
                targetCellPosition, out List<Vector3Int> path) && path.Count > skipLastMovementsCount)
            {
                nextCellPosition = path[0];
                return true;
            }

            nextCellPosition = default;
            return false;
        }
    
        private static void UpdateUnitOnRuntimeTiles(BattleBehaviour battleBehaviour, Vector3Int currentCellPosition, Vector3Int nextCellPosition)
        {
            RuntimeTile targetTileContainer = _tileRuntimeDictionary.GetContent(nextCellPosition);
            targetTileContainer.AddUnit(battleBehaviour.gameObject);

            if (_tileRuntimeDictionary.TryGetContent(currentCellPosition, out RuntimeTile previousTileBehaviour))
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
    }
}
