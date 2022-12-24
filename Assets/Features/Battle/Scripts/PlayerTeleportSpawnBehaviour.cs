using System.Collections.Generic;
using ExitGames.Client.Photon;
using Features.Mod;
using Features.Unit;
using Features.Unit.Battle.Scripts;
using Features.Unit.Classes;
using Features.Unit.GridMovement;
using Features.Unit.Modding;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Features.Battle.Scripts
{
    public class PlayerTeleportSpawnBehaviour : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        [SerializeField] private List<SpawnerInstance> spawnerInstances;
        [SerializeField] private BattleData_SO battleData;
        
        public override void OnEnable()
        {
            base.OnEnable();
        
            //ordered by logical order
            BattleManager.onLocalSpawnUnit += SpawnLocal;
            UnitMod.onAddUnit += SpawnLocal;
            UnitDragPlacementBehaviour.onPerformTeleport += RequestTeleport;
            UnitMod.onRemoveUnit += LocalDespawn;
        }

        public override void OnDisable()
        {
            base.OnDisable();
        
            //ordered by logical order
            BattleManager.onLocalSpawnUnit -= SpawnLocal;
            UnitMod.onAddUnit -= SpawnLocal;
            UnitDragPlacementBehaviour.onPerformTeleport -= RequestTeleport;
            UnitMod.onRemoveUnit -= LocalDespawn;
        }
        
        private NetworkedBattleBehaviour SpawnLocal(string spawnerReference, UnitClassData_SO unitClassData)
        {
            return SpawnHelper.SpawnUnit(spawnerInstances, PhotonNetwork.LocalPlayer.ActorNumber, spawnerReference, unitClassData, (unit, position) =>
            {
                unit.IsSpawnedLocally = true;
            });
        }
        
        private void RequestTeleport(BattleBehaviour battleBehaviour, Vector3Int targetTileGridPosition)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (battleBehaviour.IsSpawnedLocally)
                {
                    PerformTeleportThenSpawn(battleBehaviour, targetTileGridPosition,
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
                    RequestSpawnThenTeleport_RaiseEvent(battleBehaviour.PhotonView.ViewID, battleBehaviour.SpawnerInstanceIndex, battleBehaviour.UnitClassData,
                        targetTileGridPosition, PhotonNetwork.LocalPlayer.ActorNumber);
                }
                else
                {
                    RequestTeleport_RaiseEvent(battleBehaviour.NetworkedStatsBehaviour.PhotonView, targetTileGridPosition);
                }
            }
        }

        private void LocalDespawn(string spawnerReference, PhotonView photonView)
        {
            SpawnHelper.LocalDespawn(spawnerInstances, spawnerReference, photonView);
        }

        #region MasterClient Perform Methods
    
        /// <summary>
        /// Master Cast: Unit Only exists locally. Needed when a MasterClient's unit is local.
        /// </summary>
        /// <param name="battleBehaviour"></param>
        /// <param name="targetTileGridPosition"></param>
        /// <param name="unitClassData"></param>
        private void PerformTeleportThenSpawn(NetworkedBattleBehaviour battleBehaviour, Vector3Int targetTileGridPosition, UnitClassData_SO unitClassData)
        {
            Vector3Int currentCellPosition = GridPositionHelper.GetCurrentCellPosition(battleData, battleBehaviour.transform);
            if (!GridPositionHelper.IsViablePosition(battleData, targetTileGridPosition)) return;
            
            GridPositionHelper.UpdateUnitOnRuntimeTiles(battleData, battleBehaviour, currentCellPosition, targetTileGridPosition);

            //Note: the local unit will be teleported and the unit is directly spawned at the target
            TeleportObjectToTarget(battleBehaviour, targetTileGridPosition);
            PerformTeleportThenSpawn_RaiseEvent(battleBehaviour.PhotonView.ViewID, targetTileGridPosition, PhotonNetwork.LocalPlayer.ActorNumber,
                battleBehaviour.SpawnerInstanceIndex, unitClassData);
        }
        
        /// <summary>
        /// Master Cast: Unit Only exists locally. Needed when a non-MasterClient's unit is local.
        /// </summary>
        /// <param name="battleBehaviour"></param>
        /// <param name="targetTileGridPosition"></param>
        /// <param name="unitClassData"></param>
        private void PerformSpawnThenTeleport(int spawnerInstanceIndex, int actorNumber, UnitClassData_SO unitClassData, Vector3Int targetGridPosition, int viewID)
        {
            if (!GridPositionHelper.IsViablePosition(battleData, targetGridPosition)) return;
            
            SpawnerInstance spawnerInstance = spawnerInstances[spawnerInstanceIndex];
            NetworkedBattleBehaviour localUnit = spawnerInstance.InstantiateAndInitialize(actorNumber, unitClassData, targetGridPosition, spawnerInstanceIndex);

            localUnit.PhotonView.ViewID = viewID;
            localUnit.NetworkedStatsBehaviour.OnPhotonViewIdAllocated();
            
            PerformSpawnThenTeleport_RaiseEvent(localUnit, targetGridPosition);
        }
    
        /// <summary>
        /// Master Cast: Unit Already exists on all clients
        /// </summary>
        /// <param name="battleBehaviour"></param>
        /// <param name="targetCellPosition"></param>
        private void PerformGridTeleport(NetworkedBattleBehaviour battleBehaviour, Vector3Int targetCellPosition)
        {
            Vector3Int currentCellPosition = GridPositionHelper.GetCurrentCellPosition(battleData, battleBehaviour.transform);

            if (!GridPositionHelper.IsViablePosition(battleData, targetCellPosition)) return;
            
            GridPositionHelper.UpdateUnitOnRuntimeTiles(battleData, battleBehaviour, currentCellPosition, targetCellPosition);
            PerformSpawnThenTeleport_RaiseEvent(battleBehaviour, targetCellPosition);
        }

        #endregion

        #region RaiseEvents: MasterClient sends result to all
        
        private void PerformTeleportThenSpawn_RaiseEvent(int viewID, Vector3Int targetGridPosition, int photonActorNumber, int spawnerInstanceIndex, UnitClassData_SO unitClassData)
        {
            object[] data = new object[]
            {
                viewID,
                targetGridPosition,
                photonActorNumber,
                spawnerInstanceIndex,
                unitClassData
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

            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnPerformTeleportThenSpawn, data, raiseEventOptions, sendOptions);
        }

        private void PerformSpawnThenTeleport_RaiseEvent(NetworkedBattleBehaviour battleBehaviour, Vector3Int targetCellPosition)
        {
            object[] data = new object[]
            {
                battleBehaviour.PhotonView.ViewID,
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

            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnPerformSpawnThenTeleport, data, raiseEventOptions, sendOptions);
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

            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnRequestTeleport, data, raiseEventOptions, sendOptions);
        }

        private void RequestSpawnThenTeleport_RaiseEvent(int viewID, int spawnerInstanceIndex, UnitClassData_SO unitClassData, Vector3Int targetCellPosition, int actorNumber)
        {
            object[] data = new object[]
            {
                viewID,
                spawnerInstanceIndex,
                unitClassData,
                targetCellPosition,
                actorNumber
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

            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnRequestSpawnThenTeleport, data, raiseEventOptions, sendOptions);
        }
    
        #endregion

        #region Helper Functions
        
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
                case (int)RaiseEventCode.OnPerformTeleportThenSpawn:
                {
                    object[] data = (object[]) photonEvent.CustomData;

                    int viewID = (int) data[0];
                    Vector3Int gridPosition = (Vector3Int) data[1];
                    int actorNumber = (int) data[2];
                    SpawnerInstance spawnerInstance = spawnerInstances[(int) data[3]];
                    UnitClassData_SO unitClassData = (UnitClassData_SO) data[4];
                
                    if (PhotonNetwork.IsMasterClient)
                    {
                        PhotonView photonView = PhotonView.Find(viewID);
                        photonView.GetComponent<BattleBehaviour>().IsSpawnedLocally = false;
                        photonView.GetComponent<NetworkedStatsBehaviour>().OnPhotonViewIdAllocated();
                    }
                    else
                    {
                        NetworkedBattleBehaviour player = spawnerInstance.InstantiateAndInitialize(actorNumber, unitClassData, gridPosition, (int) data[3]);
                        player.PhotonView.ViewID = viewID;
                        player.NetworkedStatsBehaviour.OnPhotonViewIdAllocated();
                    }
                    break;
                }
                case (int)RaiseEventCode.OnPerformSpawnThenTeleport:
                {
                    object[] data = (object[]) photonEvent.CustomData;
                    int viewID = (int) data[0];
                    if (battleData.AllUnitsRuntimeSet.TryGetUnitByViewID(viewID, out NetworkedBattleBehaviour networkedUnitBehaviour))
                    {
                        Vector3Int nextCellPosition = (Vector3Int) data[1];

                        //the masterClient already locked the position to prevent race conditions
                        if (!PhotonNetwork.IsMasterClient)
                        {
                            Vector3Int currentCellPosition = GridPositionHelper.GetCurrentCellPosition(battleData, networkedUnitBehaviour.transform);
                            GridPositionHelper.UpdateUnitOnRuntimeTiles(battleData, networkedUnitBehaviour, currentCellPosition, nextCellPosition);
                        }
                        
                        if (networkedUnitBehaviour.IsSpawnedLocally)
                        {
                            networkedUnitBehaviour.IsSpawnedLocally = false;
                            networkedUnitBehaviour.NetworkedStatsBehaviour.OnPhotonViewIdAllocated();
                        }
                    
                        TeleportObjectToTarget(networkedUnitBehaviour, nextCellPosition);
                    }

                    break;
                }
                case (int)RaiseEventCode.OnRequestTeleport:
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
                case (int)RaiseEventCode.OnRequestSpawnThenTeleport:
                {
                    object[] data = (object[]) photonEvent.CustomData;
                    int viewID = (int) data[0];
                    int spawnerInstanceIndex = (int) data[1];
                    UnitClassData_SO unitClassData = (UnitClassData_SO) data[2];
                    Vector3Int targetGridPosition = (Vector3Int) data[3];
                    int actorNumber = (int) data[4];
                
                    PerformSpawnThenTeleport(spawnerInstanceIndex, actorNumber, unitClassData, targetGridPosition, viewID);
                    break;
                }
            }
        }
        
        #endregion
    }
}
