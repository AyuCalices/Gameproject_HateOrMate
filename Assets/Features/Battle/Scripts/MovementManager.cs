using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Features.Tiles;
using Features.Unit.Battle.Scripts;
using Features.Unit.GridMovement;
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
            UnitDragPlacementBehaviour.onPerformTeleport += RequestTeleport;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            
            MovementState.onPerformMovement -= RequestMove;
            UnitDragPlacementBehaviour.onPerformTeleport -= RequestTeleport;
        }

        private void RequestTeleport(BattleBehaviour battleBehaviour, Vector3Int targetTileGridPosition)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PerformGridTeleport(battleBehaviour, targetTileGridPosition);
            }
            else
            {
                RequestTeleport(battleBehaviour.NetworkedStatsBehaviour.PhotonView, targetTileGridPosition);
            }
        }

        private void RequestMove(BattleBehaviour battleBehaviour, Vector3Int targetTileGridPosition, int skipLastMovementsCount)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PerformGridPositionSwap(battleBehaviour, targetTileGridPosition, skipLastMovementsCount, battleBehaviour.MovementSpeed);
            }
            else
            {
                RequestGridPositionSwap(battleBehaviour.NetworkedStatsBehaviour.PhotonView, targetTileGridPosition, skipLastMovementsCount, battleBehaviour.MovementSpeed);
            }
        }
    
        private Vector3Int CurrentCellPosition(NetworkedBattleBehaviour battleBehaviour)
        {
            return battleData.TileRuntimeDictionary.GetWorldToCellPosition(battleBehaviour.transform.position);
        }

        private void PerformGridPositionSwap(NetworkedBattleBehaviour battleBehaviour, Vector3Int targetCellPosition, int skipLastMovementsCount, float movementSpeed)
        {
            Vector3Int currentCellPosition = CurrentCellPosition(battleBehaviour);
            bool wasSuccessful = TryGetNextPosition(targetCellPosition, currentCellPosition, skipLastMovementsCount,
                out Vector3Int nextCellPosition);

            if (wasSuccessful)
            {
                UpdateUnitOnRuntimeTiles(battleBehaviour, currentCellPosition, nextCellPosition);
            }

            object[] data = new object[]
            {
                battleBehaviour.NetworkedStatsBehaviour.PhotonView.ViewID,
                nextCellPosition,
                movementSpeed,
                wasSuccessful
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
        
        private void PerformGridTeleport(NetworkedBattleBehaviour battleBehaviour, Vector3Int targetCellPosition)
        {
            Vector3Int currentCellPosition = CurrentCellPosition(battleBehaviour);
            bool wasSuccessful = battleData.TileRuntimeDictionary.TryGetByGridPosition(targetCellPosition, out RuntimeTile runtimeTile) 
                                 && runtimeTile.IsPlaceable;

            if (wasSuccessful)
            {
                UpdateUnitOnRuntimeTiles(battleBehaviour, currentCellPosition, targetCellPosition);
            }

            object[] data = new object[]
            {
                battleBehaviour.NetworkedStatsBehaviour.PhotonView.ViewID,
                targetCellPosition,
                wasSuccessful
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

            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnPerformGridTeleport, data, raiseEventOptions, sendOptions);
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
    
        private void RequestGridPositionSwap(PhotonView photonView, Vector3Int targetCellPosition, int skipLastMovementsCount, float movementSpeed)
        {
            object[] data = new object[]
            {
                photonView.ViewID,
                targetCellPosition,
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

            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnRequestGridPositionSwap, data, raiseEventOptions, sendOptions);
        }
        
        private void RequestTeleport(PhotonView photonView, Vector3Int targetCellPosition)
        {
            object[] data = new object[]
            {
                photonView.ViewID,
                targetCellPosition
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

            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnRequestGridTeleport, data, raiseEventOptions, sendOptions);
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == (int)RaiseEventCode.OnPerformGridPositionSwap)
            {
                object[] data = (object[]) photonEvent.CustomData;
                int viewID = (int) data[0];
                if (battleData.AllUnitsRuntimeSet.TryGetUnitByViewID(viewID, out NetworkedBattleBehaviour networkedUnitBehaviour))
                {
                    Vector3Int nextCellPosition = (Vector3Int) data[1];
                    float movementSpeed = (float) data[2];
                    bool wasSuccessful = (bool) data[3];
                    if (!wasSuccessful)
                    {
                        networkedUnitBehaviour.ForceIdleState();
                        return;
                    }

                    if (!PhotonNetwork.IsMasterClient)
                    {
                        Vector3Int currentCellPosition = CurrentCellPosition(networkedUnitBehaviour);
                        UpdateUnitOnRuntimeTiles(networkedUnitBehaviour, currentCellPosition, nextCellPosition);
                    }
                    
                    MoveGameObjectToTarget(networkedUnitBehaviour, nextCellPosition, movementSpeed, () =>
                    {
                        if (!networkedUnitBehaviour.TryRequestAttackState() || !networkedUnitBehaviour.TryRequestMovementStateByClosestUnit() || networkedUnitBehaviour.CurrentState is not DeathState)
                        {
                            networkedUnitBehaviour.ForceIdleState();
                        }
                    });
                }
            }
        
            if (photonEvent.Code == (int)RaiseEventCode.OnRequestGridPositionSwap)
            {
                object[] data = (object[]) photonEvent.CustomData;
                int viewID = (int) data[0];
                if (battleData.AllUnitsRuntimeSet.TryGetUnitByViewID(viewID, out NetworkedBattleBehaviour networkedUnitBehaviour))
                {
                    Vector3Int targetCellPosition = (Vector3Int) data[1];
                    int skipLastMovementsCount = (int) data[2];
                    float movementSpeed = (float) data[3];
                    PerformGridPositionSwap(networkedUnitBehaviour, targetCellPosition,
                        skipLastMovementsCount, movementSpeed);
                }
            }

            if (photonEvent.Code == (int)RaiseEventCode.OnPerformGridTeleport)
            {
                object[] data = (object[]) photonEvent.CustomData;
                int viewID = (int) data[0];
                if (battleData.AllUnitsRuntimeSet.TryGetUnitByViewID(viewID, out NetworkedBattleBehaviour networkedUnitBehaviour))
                {
                    Vector3Int nextCellPosition = (Vector3Int) data[1];
                    bool wasSuccessful = (bool) data[2];
                    if (!wasSuccessful)
                    {
                        networkedUnitBehaviour.ForceIdleState();
                        return;
                    }

                    if (!PhotonNetwork.IsMasterClient)
                    {
                        Vector3Int currentCellPosition = CurrentCellPosition(networkedUnitBehaviour);
                        UpdateUnitOnRuntimeTiles(networkedUnitBehaviour, currentCellPosition, nextCellPosition);
                    }
                    
                    TeleportObjectToTarget(networkedUnitBehaviour, nextCellPosition);
                }
            }
        
            if (photonEvent.Code == (int)RaiseEventCode.OnRequestGridTeleport)
            {
                object[] data = (object[]) photonEvent.CustomData;
                int viewID = (int) data[0];
                if (battleData.AllUnitsRuntimeSet.TryGetUnitByViewID(viewID, out NetworkedBattleBehaviour networkedUnitBehaviour))
                {
                    Vector3Int targetCellPosition = (Vector3Int) data[1];
                    PerformGridTeleport(networkedUnitBehaviour, targetCellPosition);
                }
            }
        }
    
        private void MoveGameObjectToTarget(NetworkedBattleBehaviour battleBehaviour, Vector3Int nextCellPosition, float movementSpeed, Action onComplete)
        {
            if (movementSpeed <= 0)
            {
                Debug.LogWarning("The Movement Speed is to low and thus no movement is applied!");
                onComplete.Invoke();
                return;
            }
            
            Vector3 targetPosition = battleData.TileRuntimeDictionary.GetCellToWorldPosition(nextCellPosition);
            float time = Vector3.Distance(battleBehaviour.transform.position, targetPosition) / movementSpeed;
            LeanTween.move(battleBehaviour.gameObject, targetPosition, time).setOnComplete(onComplete.Invoke);
        }
        
        private void TeleportObjectToTarget(NetworkedBattleBehaviour battleBehaviour, Vector3Int nextCellPosition)
        {
            Vector3 targetPosition = battleData.TileRuntimeDictionary.GetCellToWorldPosition(nextCellPosition);
            battleBehaviour.transform.position = targetPosition;
        }
    }
}
