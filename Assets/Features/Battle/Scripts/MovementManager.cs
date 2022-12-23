using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Features.Tiles;
using Features.Unit;
using Features.Unit.Battle.Scripts;
using Features.Unit.Classes;
using Features.Unit.GridMovement;
using Photon.Pun;
using Photon.Realtime;
using ThirdParty.LeanTween.Framework;
using UnityEngine;

namespace Features.Battle.Scripts
{
    //TODO: check isSpawnedLocally
    /// <summary>
    /// In order to prevent, that two Players can place a Unit at the same GridPosition at the same
    /// time, locking tile positions is handled by the MasterClient. When a Non-MasterClient wants
    /// to swap a tile position, he always needs to send a request to the MasterClient by a RaiseEvent.
    /// Inside the Callback, a new RaiseEvent will be called to update Positions if valid.
    ///
    /// On the other hand, if a MasterClient wants to change the Position, it directly calls a RaiseEvent
    /// to update Positions if valid.
    ///
    /// Methods starting with 'Perform' represents those position swaps, and 'Requests' are RaiseEvents
    /// that should call 'Perform'
    /// </summary>
    public class MovementManager : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        [Header("References")]
        [SerializeField] private BattleData_SO battleData;

        [SerializeField] private UnitSpawner unitSpawner;

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
                if (battleBehaviour.IsSpawnedLocally)
                {
                    PerformGridSpawnTeleport(battleBehaviour, targetTileGridPosition,
                        battleBehaviour.UnitClassData);
                }
                else
                {
                    PerformGridTeleport(battleBehaviour, targetTileGridPosition);
                }
            }
            else
            {
                if (battleBehaviour.IsSpawnedLocally)
                {
                    unitSpawner.RequestTeleportAndSpawn_RaiseEvent(battleBehaviour.PhotonView.ViewID, battleBehaviour.SpawnerInstanceIndex, battleBehaviour.UnitClassData,
                        targetTileGridPosition);
                }
                else
                {
                    RequestTeleport_RaiseEvent(battleBehaviour.NetworkedStatsBehaviour.PhotonView, targetTileGridPosition);
                }
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
                RequestGridPositionSwap_RaiseEvent(battleBehaviour.NetworkedStatsBehaviour.PhotonView, targetTileGridPosition, skipLastMovementsCount, battleBehaviour.MovementSpeed);
            }
        }
        
        #region MasterClient Perform Methods
        
        /// <summary>
        /// Master Cast: Unit Only exists locally.
        /// </summary>
        /// <param name="battleBehaviour"></param>
        /// <param name="targetTileGridPosition"></param>
        /// <param name="unitClassData"></param>
        private void PerformGridSpawnTeleport(NetworkedBattleBehaviour battleBehaviour, Vector3Int targetTileGridPosition, UnitClassData_SO unitClassData)
        {
            Vector3Int currentCellPosition = GetCurrentCellPosition(battleBehaviour.transform);
            if (IsViablePosition(targetTileGridPosition))
            {
                UpdateUnitOnRuntimeTiles(battleBehaviour, currentCellPosition, targetTileGridPosition);
                battleBehaviour.IsSpawnedLocally = false;
                
                //Note: the local unit will be teleported and the unit is directly spawned at the target
                TeleportObjectToTarget(battleBehaviour, targetTileGridPosition);
                unitSpawner.PerformGridSpawn_RaiseEvent(battleBehaviour.PhotonView.ViewID, targetTileGridPosition, PhotonNetwork.IsMasterClient,
                    battleBehaviour.SpawnerInstanceIndex, unitClassData);
            }
        }
        
        /// <summary>
        /// Master Cast: Unit Already exists on all clients
        /// </summary>
        /// <param name="battleBehaviour"></param>
        /// <param name="targetCellPosition"></param>
        private void PerformGridTeleport(NetworkedBattleBehaviour battleBehaviour, Vector3Int targetCellPosition)
        {
            Vector3Int currentCellPosition = GetCurrentCellPosition(battleBehaviour.transform);

            if (!IsViablePosition(targetCellPosition)) return;
            
            UpdateUnitOnRuntimeTiles(battleBehaviour, currentCellPosition, targetCellPosition);
            PerformGridTeleport_RaiseEvent(battleBehaviour, targetCellPosition);
        }

        /// <summary>
        /// Master Cast
        /// </summary>
        /// <param name="battleBehaviour"></param>
        /// <param name="targetTileGridPosition"></param>
        /// <param name="skipLastMovementsCount"></param>
        /// <param name="movementSpeed"></param>
        private void PerformGridPositionSwap(NetworkedBattleBehaviour battleBehaviour, Vector3Int targetTileGridPosition, int skipLastMovementsCount, float movementSpeed)
        {
            Vector3Int currentCellPosition = GetCurrentCellPosition(battleBehaviour.transform);
            if (!TryGetNextPathfindingPosition(targetTileGridPosition, currentCellPosition, skipLastMovementsCount,
                out Vector3Int nextCellPosition)) return;
                
            UpdateUnitOnRuntimeTiles(battleBehaviour, currentCellPosition, nextCellPosition);
            PerformGridPositionSwap_RaiseEvent(battleBehaviour, nextCellPosition, movementSpeed);
        }
        
        #endregion

        
        #region RaiseEvents: Requests for MasterClient

        private void RequestTeleport_RaiseEvent(PhotonView photonView, Vector3Int targetCellPosition)
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
    
        private void RequestGridPositionSwap_RaiseEvent(PhotonView photonView, Vector3Int targetCellPosition, int skipLastMovementsCount, float movementSpeed)
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

        #endregion
        
        
        #region RaiseEvents: MasterClient sends result to all
        
        public void PerformGridTeleport_RaiseEvent(NetworkedBattleBehaviour battleBehaviour, Vector3Int targetCellPosition)
        {
            object[] data = new object[]
            {
                battleBehaviour.NetworkedStatsBehaviour.PhotonView.ViewID,
                targetCellPosition
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
        
        private void PerformGridPositionSwap_RaiseEvent(NetworkedBattleBehaviour battleBehaviour, Vector3Int nextCellPosition, float movementSpeed)
        {
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
        
        #endregion
        
        
        #region Helper Functions

        private Vector3Int GetCurrentCellPosition(Transform currentTransform)
        {
            return battleData.TileRuntimeDictionary.GetWorldToCellPosition(currentTransform.position);
        }

        public bool IsViablePosition(Vector3Int targetCellPosition)
        {
            return battleData.TileRuntimeDictionary.TryGetByGridPosition(targetCellPosition, out RuntimeTile runtimeTile) 
                   && runtimeTile.IsPlaceable;
        }
        
        private bool TryGetNextPathfindingPosition(Vector3Int targetCellPosition, Vector3Int startCellPosition, int skipLastMovementsCount, out Vector3Int nextCellPosition)
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
        
        #endregion
        
        
        #region RaiseEvent Callbacks
        
        public void OnEvent(EventData photonEvent)
        {
            switch (photonEvent.Code)
            {
                case (int)RaiseEventCode.OnPerformGridPositionSwap:
                {
                    object[] data = (object[]) photonEvent.CustomData;
                    int viewID = (int) data[0];
                    if (battleData.AllUnitsRuntimeSet.TryGetUnitByViewID(viewID, out NetworkedBattleBehaviour networkedUnitBehaviour))
                    {
                        Vector3Int nextCellPosition = (Vector3Int) data[1];
                        float movementSpeed = (float) data[2];

                        //the masterClient already locked the position to prevent race conditions
                        if (!PhotonNetwork.IsMasterClient)
                        {
                            Vector3Int currentCellPosition = GetCurrentCellPosition(networkedUnitBehaviour.transform);
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

                    break;
                }
                case (int)RaiseEventCode.OnRequestGridPositionSwap:
                {
                    object[] data = (object[]) photonEvent.CustomData;
                    int viewID = (int) data[0];
                    if (battleData.AllUnitsRuntimeSet.TryGetUnitByViewID(viewID, out NetworkedBattleBehaviour networkedUnitBehaviour))
                    {
                        Vector3Int targetCellPosition = (Vector3Int) data[1];
                        int skipLastMovementsCount = (int) data[2];
                        float movementSpeed = (float) data[3];

                        PerformGridPositionSwap(networkedUnitBehaviour, targetCellPosition, skipLastMovementsCount, movementSpeed);
                    }

                    break;
                }
                case (int)RaiseEventCode.OnPerformGridTeleport:
                {
                    object[] data = (object[]) photonEvent.CustomData;
                    int viewID = (int) data[0];
                    if (battleData.AllUnitsRuntimeSet.TryGetUnitByViewID(viewID, out NetworkedBattleBehaviour networkedUnitBehaviour))
                    {
                        Vector3Int nextCellPosition = (Vector3Int) data[1];

                        networkedUnitBehaviour.IsSpawnedLocally = false;

                        //the masterClient already locked the position to prevent race conditions
                        if (!PhotonNetwork.IsMasterClient)
                        {
                            Vector3Int currentCellPosition = GetCurrentCellPosition(networkedUnitBehaviour.transform);
                            UpdateUnitOnRuntimeTiles( networkedUnitBehaviour, currentCellPosition, nextCellPosition);
                        }
                    
                        TeleportObjectToTarget(networkedUnitBehaviour, nextCellPosition);
                    }

                    break;
                }
                case (int)RaiseEventCode.OnRequestGridTeleport:
                {
                    object[] data = (object[]) photonEvent.CustomData;
                    int viewID = (int) data[0];

                    if (battleData.AllUnitsRuntimeSet.TryGetUnitByViewID(viewID, out NetworkedBattleBehaviour networkedUnitBehaviour))
                    {
                        Vector3Int targetCellPosition = (Vector3Int) data[1];
                        PerformGridTeleport(networkedUnitBehaviour, targetCellPosition);
                    }

                    break;
                }
            }
        }
        
        #endregion

    }
}
