using System;
using System.Collections.Generic;
using Features.Battle.Scripts;
using Features.Tiles;
using Features.Unit.Modding;
using UnityEngine;

namespace Features.Unit
{
    public class SpawnerInstance : MonoBehaviour
    {
        [SerializeField] private BattleData_SO battleData;
        [SerializeField] private SpawnPosition spawnPosition;
        
        public string reference;
        public GameObject localPlayerPrefab;
        public GameObject networkedPlayerPrefab;
        public bool isTargetable;

        private List<NetworkedUnitBehaviour> _spawnedUnits = new List<NetworkedUnitBehaviour>();

        public bool TryGetSpawnPosition(out KeyValuePair<Vector3Int, RuntimeTile> tileKeyValuePair)
        {
            switch (spawnPosition)
            {
                case SpawnPosition.RandomPlaceablePosition:
                    if (battleData.TileRuntimeDictionary.TryGetRandomPlaceableTileBehaviour(
                        out tileKeyValuePair)) return true;
                    break;
                
                case SpawnPosition.ThisTransform:
                    Vector3Int gridPosition = battleData.TileRuntimeDictionary.GetWorldToCellPosition(transform.position);

                    if (battleData.TileRuntimeDictionary.TryGetByGridPosition(gridPosition,
                        out RuntimeTile runtimeTile) && !runtimeTile.ContainsUnit)
                    {
                        tileKeyValuePair = new KeyValuePair<Vector3Int, RuntimeTile>(gridPosition, runtimeTile);
                        return true;
                    }

                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }

            tileKeyValuePair = default;
            return false;
        }

        public void Spawn()
        {
            
        }
    }
}
