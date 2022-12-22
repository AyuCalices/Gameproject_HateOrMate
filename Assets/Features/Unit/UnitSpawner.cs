using System.Collections.Generic;
using ExitGames.Client.Photon;
using Features.Battle.Scripts;
using Features.Experimental;
using Features.Mod;
using Features.Tiles;
using Features.Unit.Battle.Scripts;
using Features.Unit.Classes;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Features.Unit
{
    //TODO: cleanup
    public class UnitSpawner : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        [SerializeField] private BattleData_SO battleData;
        [SerializeField] private List<SpawnerInstance> spawnerInstances;

        public override void OnEnable()
        {
            base.OnEnable();
            TestingGenerator.onNetworkedSpawnUnit += NetworkedSpawn;
            BattleManager.onNetworkedSpawnUnit += NetworkedSpawn;
            BattleManager.onLocalDespawnAllUnits += NetworkedDespawnAll;
            BattleManager.onLocalSpawnUnit += SpawnLocal;
            UnitMod.onAddUnit += SpawnLocal;
            UnitMod.onRemoveUnit += LocalDespawn;
            MovementManager.onRequestTeleportAndSpawn += RequestTeleportAndSpawn;
            MovementManager.onPerformTeleportAndSpawn += PerformSpawnRaiseEvent;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            TestingGenerator.onNetworkedSpawnUnit -= NetworkedSpawn;
            BattleManager.onNetworkedSpawnUnit -= NetworkedSpawn;
            BattleManager.onLocalDespawnAllUnits -= NetworkedDespawnAll;
            BattleManager.onLocalSpawnUnit -= SpawnLocal;
            UnitMod.onAddUnit -= SpawnLocal;
            UnitMod.onRemoveUnit -= LocalDespawn;
            MovementManager.onRequestTeleportAndSpawn -= RequestTeleportAndSpawn;
            MovementManager.onPerformTeleportAndSpawn -= PerformSpawnRaiseEvent;
        }

        private NetworkedBattleBehaviour SpawnLocal(string spawnerReference, UnitClassData_SO unitClassData)
        {
            int spawnerInstanceIndex = spawnerInstances.FindIndex(x => x.reference == spawnerReference);
            if (spawnerInstanceIndex == -1)
            {
                Debug.LogError("Spawner Instance was not Found!");
            }
            
            if (!spawnerInstances[spawnerInstanceIndex]
                .TryGetSpawnPosition(out KeyValuePair<Vector3Int, RuntimeTile> tileKeyValuePair))
            {
                return null;
            }
            
            SpawnerInstance spawnerInstance = spawnerInstances[spawnerInstanceIndex];
            NetworkedBattleBehaviour selectedPrefab = spawnerInstance.localPlayerPrefab;
            UnitTeamData_SO unitTeamData = spawnerInstance.localPlayerTeamData;
            NetworkedBattleBehaviour localUnit = spawnerInstance.InstantiateAndInitialize(selectedPrefab, unitTeamData, unitClassData, tileKeyValuePair.Key, spawnerInstanceIndex);

            if (localUnit == null) return null;

            if (PhotonNetwork.AllocateViewID(localUnit.PhotonView))
            {
                localUnit.NetworkedStatsBehaviour.OnPhotonViewIdAllocated();
                localUnit.IsSpawnedLocally = true;
                return localUnit;
            }
            
            Debug.LogError("Failed to allocate a ViewId.");
            spawnerInstances[spawnerInstanceIndex].DestroyByReference(localUnit.PhotonView);
            return null;
        }

        private NetworkedBattleBehaviour SpawnNetworkedIfNotExisting(int spawnerInstanceIndex, UnitClassData_SO unitClassData, Vector3Int targetGridPosition, int viewID)
        {
            SpawnerInstance spawnerInstance = spawnerInstances[spawnerInstanceIndex];
            NetworkedBattleBehaviour selectedPrefab = spawnerInstance.networkedPlayerPrefab;
            UnitTeamData_SO unitTeamData = spawnerInstance.networkedPlayerTeamData;
            NetworkedBattleBehaviour localUnit = spawnerInstance.InstantiateAndInitialize(selectedPrefab, unitTeamData, unitClassData, targetGridPosition, spawnerInstanceIndex);

            if (localUnit == null) return null;

            localUnit.PhotonView.ViewID = viewID;
            localUnit.NetworkedStatsBehaviour.OnPhotonViewIdAllocated();
            return localUnit;
        }

        private void LocalDespawn(string spawnerReference, PhotonView photonView)
        {
            int spawnerInstanceIndex = spawnerInstances.FindIndex(x => x.reference == spawnerReference);
            if (spawnerInstanceIndex == -1)
            {
                Debug.LogError("Spawner Instance was not Found!");
            }
            
            spawnerInstances[spawnerInstanceIndex].DestroyByReference(photonView);
        }

        private void NetworkedDespawnAll(string spawnerReference)
        {
            int spawnerInstanceIndex = spawnerInstances.FindIndex(x => x.reference == spawnerReference);
            if (spawnerInstanceIndex == -1)
            {
                Debug.LogError("Spawner Instance was not Found!");
            }
            
            spawnerInstances[spawnerInstanceIndex].DestroyAll();
        }

        private void NetworkedSpawn(string spawnerReference, UnitClassData_SO unitClassData)
        {
            int spawnerInstanceIndex = spawnerInstances.FindIndex(x => x.reference == spawnerReference);
            if (spawnerInstanceIndex == -1)
            {
                Debug.LogError("Spawner Instance was not Found!");
            }
            
            if (PhotonNetwork.IsMasterClient)
            {
                SpawnUnit(PhotonNetwork.IsMasterClient, spawnerInstanceIndex, unitClassData);
            }
            else
            {
                RequestSpawnRaiseEvent(PhotonNetwork.IsMasterClient, spawnerInstanceIndex, unitClassData);
            }
        }
        
        private NetworkedBattleBehaviour SpawnUnit(bool isSpawnedByMaster, int spawnerInstanceIndex, UnitClassData_SO unitClassData)
        {
            if (!spawnerInstances[spawnerInstanceIndex]
                .TryGetSpawnPosition(out KeyValuePair<Vector3Int, RuntimeTile> tileKeyValuePair))
            {
                return null;
            }

            SpawnerInstance spawnerInstance = spawnerInstances[spawnerInstanceIndex];
            NetworkedBattleBehaviour selectedPrefab = isSpawnedByMaster ? spawnerInstance.localPlayerPrefab : spawnerInstance.networkedPlayerPrefab;
            UnitTeamData_SO unitTeamData = isSpawnedByMaster ? spawnerInstance.localPlayerTeamData : spawnerInstance.networkedPlayerTeamData;
            NetworkedBattleBehaviour player = spawnerInstance.InstantiateAndInitialize(selectedPrefab, unitTeamData, unitClassData, tileKeyValuePair.Key, spawnerInstanceIndex);

            MasterSpawnRaiseEvent(player, tileKeyValuePair.Key, isSpawnedByMaster, spawnerInstanceIndex, unitClassData);

            return player;
        }

        private void MasterSpawnRaiseEvent(NetworkedBattleBehaviour playerPrefab, Vector3Int gridPosition, bool isSpawnedByMaster, int spawnerInstanceIndex, UnitClassData_SO unitClassData)
        {
            if (PhotonNetwork.AllocateViewID(playerPrefab.PhotonView))
            {
                PerformSpawnRaiseEvent(playerPrefab.PhotonView.ViewID, gridPosition, isSpawnedByMaster,
                    spawnerInstanceIndex, unitClassData);
                playerPrefab.NetworkedStatsBehaviour.OnPhotonViewIdAllocated();
            }
            else
            {
                Debug.LogError("Failed to allocate a ViewId.");

                spawnerInstances[spawnerInstanceIndex].DestroyByReference(playerPrefab.PhotonView);
            }
        }

        private void PerformSpawnRaiseEvent(int viewID, Vector3Int gridPosition, bool isSpawnedByMaster, int spawnerInstanceIndex, UnitClassData_SO unitClassData)
        {
            object[] data = new object[]
            {
                viewID,
                gridPosition,
                isSpawnedByMaster,
                spawnerInstanceIndex,
                unitClassData
            };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.Others,
                CachingOption = EventCaching.AddToRoomCache
            };

            SendOptions sendOptions = new SendOptions
            {
                Reliability = true
            };

            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnUnitManualInstantiation, data, raiseEventOptions, sendOptions);
        }
        
        private void RequestSpawnRaiseEvent(bool isSpawnedByMaster, int spawnerInstanceIndex, UnitClassData_SO unitClassData)
        {
            object[] data = new object[]
            {
                isSpawnedByMaster,
                spawnerInstanceIndex,
                unitClassData
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

            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnRequestUnitManualInstantiation, data, raiseEventOptions, sendOptions);
        }
        
        private void RequestTeleportAndSpawn(int viewID, int spawnerInstanceIndex, UnitClassData_SO unitClassData, Vector3Int targetCellPosition)
        {
            object[] data = new object[]
            {
                viewID,
                spawnerInstanceIndex,
                unitClassData,
                targetCellPosition
            };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.Others,
                CachingOption = EventCaching.AddToRoomCache
            };

            SendOptions sendOptions = new SendOptions
            {
                Reliability = true
            };

            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnRequestGridTeleportAndSpawn, data, raiseEventOptions, sendOptions);
        }
        
        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == (int)RaiseEventCode.OnUnitManualInstantiation)
            {
                object[] data = (object[]) photonEvent.CustomData;

                Vector3Int gridPosition = (Vector3Int) data[1];
                bool isSpawnedByMaster = (bool) data[2];
                SpawnerInstance spawnerInstance = spawnerInstances[(int) data[3]];
                UnitClassData_SO unitClassData = (UnitClassData_SO) data[4];

                NetworkedBattleBehaviour selectedPrefab = isSpawnedByMaster ? spawnerInstance.networkedPlayerPrefab : spawnerInstance.localPlayerPrefab;
                UnitTeamData_SO unitTeamData = isSpawnedByMaster ? spawnerInstance.networkedPlayerTeamData : spawnerInstance.localPlayerTeamData;
                NetworkedBattleBehaviour player = spawnerInstance.InstantiateAndInitialize(selectedPrefab, unitTeamData, unitClassData, gridPosition, (int) data[3]);
                
                player.PhotonView.ViewID = (int) data[0];
                player.NetworkedStatsBehaviour.OnPhotonViewIdAllocated();
            }

            if (photonEvent.Code == (int)RaiseEventCode.OnRequestUnitManualInstantiation)
            {
                object[] data = (object[]) photonEvent.CustomData;
                SpawnUnit((bool) data[0], (int) data[1], (UnitClassData_SO) data[2]);
            }
            
            if (photonEvent.Code == (int)RaiseEventCode.OnRequestGridTeleportAndSpawn)
            {
                object[] data = (object[]) photonEvent.CustomData;
                int viewID = (int) data[0];
                int spawnerInstanceIndex = (int) data[1];
                UnitClassData_SO unitClassData = (UnitClassData_SO) data[2];
                Vector3Int targetGridPosition = (Vector3Int) data[3];

                bool isViablePosition = MovementManager.IsViablePosition(battleData, targetGridPosition);

                if (isViablePosition)
                {
                    NetworkedBattleBehaviour battleBehaviour = SpawnNetworkedIfNotExisting(spawnerInstanceIndex, unitClassData, targetGridPosition, viewID);
                    MovementManager.PerformGridTeleport(battleBehaviour, targetGridPosition, isViablePosition);
                }
            }
        }
    }
}
