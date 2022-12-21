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
    //TODO: see below
    //spawn events: player gets mod -> instantiation till mod removed -> then destroy (needs reference on mod)
    //spawn events: ai enemy dependant on stage -> enable when stage starts -> then disable from pool ?
    
    //for each stage: one stage spawner can max spawn 1 unit. -> inside the stage definition/generation
    //for each player: there are multiple units that can be placed on the grid -> only on spots with space
    //each spawner has references of the spawned units
    //when a unit gets spawned, it needs to get all the modifiers
    //pooling for units => mods will be assigned for each of them even if it is a inactive pool object
    //every pooled object that gets inactive needs to be in deathState
    
    //mod => keeps reference of spawned unit
    //stage => keeps reference of spawned unit
    //must only be saved by owner => for destroying: owner sends RaiseEvent
    
    //TODO: new
    //how to destroy units ? how to setup stages ? how to add battle actions ?
    
    public class UnitSpawner : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        [SerializeField] private List<SpawnerInstance> spawnerInstances;

        public override void OnEnable()
        {
            base.OnEnable();
            TestingGenerator.onNetworkedSpawnUnit += NetworkedSpawn;
            BattleManager.onNetworkedSpawnUnit += NetworkedSpawn;
            BattleManager.onLocalDespawnAllUnits += NetworkedDespawnAll;
            UnitMod.onAddUnit += SpawnLocal;
            UnitMod.onRemoveUnit += LocalDespawn;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            TestingGenerator.onNetworkedSpawnUnit -= NetworkedSpawn;
            BattleManager.onNetworkedSpawnUnit -= NetworkedSpawn;
            BattleManager.onLocalDespawnAllUnits -= NetworkedDespawnAll;
            UnitMod.onAddUnit -= SpawnLocal;
            UnitMod.onRemoveUnit -= LocalDespawn;
        }

        private NetworkedBattleBehaviour SpawnLocal(string spawnerReference, UnitClassData_SO unitClassData)
        {
            int spawnerInstanceIndex = spawnerInstances.FindIndex(x => x.reference == spawnerReference);
            if (spawnerInstanceIndex == -1)
            {
                Debug.LogError("Spawner Instance was not Found!");
            }

            NetworkedBattleBehaviour localUnit = SpawnUnit(PhotonNetwork.IsMasterClient, spawnerInstanceIndex, unitClassData,false);

            if (localUnit == null) return null;

            if (localUnit.NetworkedStatsBehaviour.NetworkingInitialized) return localUnit;
            
            if (PhotonNetwork.AllocateViewID(localUnit.PhotonView))
            {
                localUnit.NetworkedStatsBehaviour.OnPhotonViewIdAllocated();
                return localUnit;
            }
            
            Debug.LogError("Failed to allocate a ViewId.");
            spawnerInstances[spawnerInstanceIndex].DestroyByReference(localUnit.PhotonView);
            return null;
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
                SpawnUnit(PhotonNetwork.IsMasterClient, spawnerInstanceIndex, unitClassData, true);
            }
            else
            {
                RequestSpawnRaiseEvent(PhotonNetwork.IsMasterClient, spawnerInstanceIndex, unitClassData);
            }
        }
        
        private NetworkedBattleBehaviour SpawnUnit(bool isSpawnedByMaster, int spawnerInstanceIndex, UnitClassData_SO unitClassData, bool castRaiseEvent)
        {
            if (!spawnerInstances[spawnerInstanceIndex]
                .TryGetSpawnPosition(out KeyValuePair<Vector3Int, RuntimeTile> tileKeyValuePair))
            {
                return null;
            }

            SpawnerInstance spawnerInstance = spawnerInstances[spawnerInstanceIndex];
            NetworkedBattleBehaviour selectedPrefab = isSpawnedByMaster ? spawnerInstance.localPlayerPrefab : spawnerInstance.networkedPlayerPrefab;
            UnitTeamData_SO unitTeamData = isSpawnedByMaster ? spawnerInstance.localPlayerTeamData : spawnerInstance.networkedPlayerTeamData;
            NetworkedBattleBehaviour player = spawnerInstance.InstantiateAndInitialize(selectedPrefab, unitTeamData, unitClassData, tileKeyValuePair.Key);

            if (castRaiseEvent)
            {
                MasterSpawnRaiseEvent(player, tileKeyValuePair.Key, isSpawnedByMaster, spawnerInstanceIndex, unitClassData);
            }

            return player;
        }

        private void MasterSpawnRaiseEvent(NetworkedBattleBehaviour playerPrefab, Vector3Int gridPosition, bool isSpawnedByMaster, int spawnerInstanceIndex, UnitClassData_SO unitClassData)
        {
            if (playerPrefab.NetworkedStatsBehaviour.NetworkingInitialized)
            {
                PerformSpawnRaiseEvent(playerPrefab.PhotonView.ViewID, gridPosition, isSpawnedByMaster,
                    spawnerInstanceIndex, unitClassData);
                return;
            }
            
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
                NetworkedBattleBehaviour player = spawnerInstance.InstantiateAndInitialize(selectedPrefab, unitTeamData, unitClassData, gridPosition);
                
                player.PhotonView.ViewID = (int) data[0];
                player.NetworkedStatsBehaviour.OnPhotonViewIdAllocated();
            }

            if (photonEvent.Code == (int)RaiseEventCode.OnRequestUnitManualInstantiation)
            {
                object[] data = (object[]) photonEvent.CustomData;
                SpawnUnit((bool) data[0], (int) data[1], (UnitClassData_SO) data[2], true);
            }
        }
    }
}
