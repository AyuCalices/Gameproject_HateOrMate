using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Features.Battle.Scripts;
using Features.Experimental;
using Features.Tiles;
using Features.Unit.Battle.Scripts;
using Features.Unit.Modding;
using Photon.Pun;
using Photon.Realtime;
using ToolBox.Pools;
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
    
    public enum SpawnPosition { RandomPlaceablePosition, ThisTransform }
    public class UnitSpawner : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        [SerializeField] private BattleData_SO battleData;

        [SerializeField] private List<SpawnerInstance> spawnerInstances;

        public override void OnEnable()
        {
            base.OnEnable();
            TestingGenerator.onSpawnUnit += Spawn;
            StageSetupState.onSpawnUnit += Spawn;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            TestingGenerator.onSpawnUnit -= Spawn;
            StageSetupState.onSpawnUnit -= Spawn;
        }

        private void Spawn(string spawnerReference)
        {
            int spawnerInstanceIndex = spawnerInstances.FindIndex(x => x.reference == spawnerReference);
            if (spawnerInstanceIndex == -1)
            {
                Debug.LogError("Spawner Instance was not Found!");
            }
            
            if (PhotonNetwork.IsMasterClient)
            {
                SpawnUnit(PhotonNetwork.IsMasterClient, spawnerInstanceIndex);
            }
            else
            {
                RequestSpawnRaiseEvent(PhotonNetwork.IsMasterClient, spawnerInstanceIndex);
            }
        }

        private void DestroyLocal(int viewID)
        {
            if (!battleData.AllUnitsRuntimeSet.TryGetUnitByViewID(viewID,
                out NetworkedUnitBehaviour networkedUnitBehaviour)) return;
            
            networkedUnitBehaviour.gameObject.Release();
        }

        private void SpawnUnit(bool isSpawnedByMaster, int spawnerInstanceIndex)
        {
            if (!spawnerInstances[spawnerInstanceIndex]
                .TryGetSpawnPosition(out KeyValuePair<Vector3Int, RuntimeTile> tileKeyValuePair)) return;

            GameObject selectedPrefab = isSpawnedByMaster ? spawnerInstances[spawnerInstanceIndex].localPlayerPrefab : spawnerInstances[spawnerInstanceIndex].networkedPlayerPrefab;
            GameObject player = selectedPrefab.Reuse(transform);
            player.GetComponent<NetworkedBattleBehaviour>().IsTargetable = spawnerInstances[spawnerInstanceIndex].isTargetable;
            tileKeyValuePair.Value.AddUnit(player);
            player.transform.position = battleData.TileRuntimeDictionary.GetCellToWorldPosition(tileKeyValuePair.Key);

            MasterSpawnRaiseEvent(player, tileKeyValuePair.Key, isSpawnedByMaster, spawnerInstanceIndex);
        }

        private void MasterSpawnRaiseEvent(GameObject playerPrefab, Vector3Int gridPosition, bool isSpawnedByMaster, int spawnerInstanceIndex)
        {
            //TODO: getComponent
            PhotonView instantiationPhotonView = playerPrefab.GetComponent<PhotonView>();
            if (PhotonNetwork.AllocateViewID(instantiationPhotonView))
            {
                var playerTransform = playerPrefab.transform;
                object[] data = new object[]
                {
                    playerTransform.position, playerTransform.rotation, instantiationPhotonView.ViewID, gridPosition, isSpawnedByMaster, spawnerInstanceIndex
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
                //TODO: getComponent
                playerPrefab.GetComponent<NetworkedUnitBehaviour>().OnPhotonViewIdAllocated();
            }
            else
            {
                Debug.LogError("Failed to allocate a ViewId.");

                Destroy(playerPrefab);
            }
        }
        
        private void RequestSpawnRaiseEvent(bool isSpawnedByMaster, int spawnerInstanceIndex)
        {
            object[] data = new object[]
            {
                isSpawnedByMaster,
                spawnerInstanceIndex
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

                //instantiate and prepare photon view
                SpawnerInstance spawnerInstance = spawnerInstances[(int) data[5]];
                GameObject selectedPrefab = (bool) data[4] ? spawnerInstance.networkedPlayerPrefab : spawnerInstance.localPlayerPrefab;
                GameObject player = selectedPrefab.Reuse((Vector3) data[0], (Quaternion) data[1], transform);
                
                player.GetComponent<NetworkedBattleBehaviour>().IsTargetable = spawnerInstance.isTargetable;
                //TODO: getComponent
                PhotonView instantiationPhotonView = player.GetComponent<PhotonView>();
                instantiationPhotonView.ViewID = (int) data[2];

                //sed grid position for all clients
                if (battleData.TileRuntimeDictionary.TryGetByGridPosition((Vector3Int) data[3], out RuntimeTile tileBehaviour))
                {
                    tileBehaviour.AddUnit(player);
                }
                
                //TODO: getComponent
                player.GetComponent<NetworkedUnitBehaviour>().OnPhotonViewIdAllocated();
            }

            if (photonEvent.Code == (int)RaiseEventCode.OnRequestUnitManualInstantiation)
            {
                object[] data = (object[]) photonEvent.CustomData;
                SpawnUnit((bool) data[0], (int) data[1]);
            }
        }
    }
}
