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
        public static UnitBattleBehaviour PhotonSpawnUnit(List<SpawnerInstance> spawnerInstances, string spawnerReference, 
            UnitClassData_SO unitClassData, int level, bool isBenched)
        {
            int spawnerInstanceIndex = GetSpawnerInstanceIndex(spawnerInstances, spawnerReference);
            
            if (!spawnerInstances[spawnerInstanceIndex]
                .TryGetSpawnPosition(out KeyValuePair<Vector3Int, RuntimeTile> tileKeyValuePair)) return null;

            SpawnerInstance spawnerInstance = spawnerInstances[spawnerInstanceIndex];
            return spawnerInstance.PhotonInstantiate(unitClassData, tileKeyValuePair.Key, level, isBenched);
        }

        private static int GetSpawnerInstanceIndex(List<SpawnerInstance> spawnerInstances, string spawnerReference)
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
