using System;
using System.Collections.Generic;
using Features.Tiles;
using Features.Unit;
using Features.Unit.Battle.Scripts;
using Features.Unit.Classes;
using Photon.Pun;
using UnityEngine;

namespace Features.Battle.Scripts
{
    public static class SpawnHelper
    {
        public static void LocalDespawn(List<SpawnerInstance> spawnerInstances, string spawnerReference, PhotonView photonView)
        {
            int spawnerInstanceIndex = GetSpawnerInstanceIndex(spawnerInstances, spawnerReference);
            spawnerInstances[spawnerInstanceIndex].DestroyByReference(photonView);
        }
        
        public static NetworkedBattleBehaviour SpawnUnit(List<SpawnerInstance> spawnerInstances, int photonActorNumber, string spawnerReference, UnitClassData_SO unitClassData, Action<NetworkedBattleBehaviour, Vector3Int> onSpawnSuccessful)
        {
            int spawnerInstanceIndex = GetSpawnerInstanceIndex(spawnerInstances, spawnerReference);
            
            if (!spawnerInstances[spawnerInstanceIndex]
                .TryGetSpawnPosition(out KeyValuePair<Vector3Int, RuntimeTile> tileKeyValuePair)) return null;

            SpawnerInstance spawnerInstance = spawnerInstances[spawnerInstanceIndex];
            NetworkedBattleBehaviour player = spawnerInstance.InstantiateAndInitialize(photonActorNumber, unitClassData, tileKeyValuePair.Key, spawnerInstanceIndex);

            if (PhotonNetwork.AllocateViewID(player.PhotonView))
            {
                onSpawnSuccessful.Invoke(player, tileKeyValuePair.Key);
                return player;
            }
            
            Debug.LogError("Failed to allocate a ViewId.");
            spawnerInstances[spawnerInstanceIndex].DestroyByReference(player.PhotonView);
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
        
        public static void PlayerSynchronizedDespawnAll(List<SpawnerInstance> spawnerInstances, string spawnerReference)
        {
            int spawnerInstanceIndex = GetSpawnerInstanceIndex(spawnerInstances, spawnerReference);
            spawnerInstances[spawnerInstanceIndex].DestroyAll();
        }
    }
}
