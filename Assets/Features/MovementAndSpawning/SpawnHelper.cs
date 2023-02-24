using System;
using System.Collections.Generic;
using Features.Tiles.Scripts;
using Features.Unit.Scripts;
using Features.Unit.Scripts.Behaviours.Battle;
using Photon.Pun;
using UnityEngine;

namespace Features.MovementAndSpawning
{
    public static class SpawnHelper
    {
        public static NetworkedBattleBehaviour PhotonSpawnUnit(List<SpawnerInstance> spawnerInstances, int photonActorNumber, string spawnerReference, 
            UnitClassData_SO unitClassData, int level)
        {
            int spawnerInstanceIndex = GetSpawnerInstanceIndex(spawnerInstances, spawnerReference);
            
            if (!spawnerInstances[spawnerInstanceIndex]
                .TryGetSpawnPosition(out KeyValuePair<Vector3Int, RuntimeTile> tileKeyValuePair)) return null;

            SpawnerInstance spawnerInstance = spawnerInstances[spawnerInstanceIndex];
            return spawnerInstance.PhotonInstantiate(photonActorNumber, unitClassData, tileKeyValuePair.Key, spawnerInstanceIndex, level);
        }
        
        public static NetworkedBattleBehaviour SpawnUnit(List<SpawnerInstance> spawnerInstances, int photonActorNumber, string spawnerReference, 
            UnitClassData_SO unitClassData, int level, Action<NetworkedBattleBehaviour, Vector3Int> onSpawnSuccessful)
        {
            int spawnerInstanceIndex = GetSpawnerInstanceIndex(spawnerInstances, spawnerReference);
            
            if (!spawnerInstances[spawnerInstanceIndex]
                .TryGetSpawnPosition(out KeyValuePair<Vector3Int, RuntimeTile> tileKeyValuePair)) return null;

            SpawnerInstance spawnerInstance = spawnerInstances[spawnerInstanceIndex];
            NetworkedBattleBehaviour player = spawnerInstance.InstantiateAndInitialize(photonActorNumber, unitClassData, tileKeyValuePair.Key, spawnerInstanceIndex, level);

            if (PhotonNetwork.AllocateViewID(player.PhotonView))
            {
                onSpawnSuccessful.Invoke(player, tileKeyValuePair.Key);
                return player;
            }
            
            Debug.LogError("Failed to allocate a ViewId.");
            player.Destroy();
            return null;
        }
        
        public static int GetSpawnerInstanceIndex(List<SpawnerInstance> spawnerInstances, string spawnerReference)
        {
            int spawnerInstanceIndex = spawnerInstances.FindIndex(x => x.reference == spawnerReference);
            if (spawnerInstanceIndex == -1)
            {
                Debug.LogError("Spawner Instance was not Found!");
                return -1;
            }

            return spawnerInstanceIndex;
        }
    }
}
