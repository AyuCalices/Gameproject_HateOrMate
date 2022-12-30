using System.Collections.Generic;
using Features.Battle.Scripts;
using Features.MovementAndSpawning;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Features.Tiles.Scripts
{
    public class TileManager : MonoBehaviour
    {
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private BattleData_SO battleData;
        [SerializeField] private TileLookup[] tileReferences;
        [SerializeField] private List<SpawnerInstance> spawnerInstances;

        private void Awake()
        {
            battleData.TileRuntimeDictionary.ClearContainedUnits();
            battleData.TileRuntimeDictionary.Initialize(tilemap, tileReferences, PopulateTileRuntimeDictionary);
        }

        private void PopulateTileRuntimeDictionary(Dictionary<Vector3Int, RuntimeTile> tileDictionary)
        {
            Dictionary<Vector3Int, SpawnPosition> spawnerGridPositions = new Dictionary<Vector3Int, SpawnPosition>();
            foreach (SpawnerInstance spawner in spawnerInstances)
            {
                foreach (SpawnPosition spawnPosition in spawner.spawnPositions)
                {
                    spawnerGridPositions.Add(tilemap.WorldToCell(spawnPosition.transform.position), spawnPosition);
                }
            }
            
            foreach (Vector3Int position in tilemap.cellBounds.allPositionsWithin)
            {
                foreach (TileLookup tileData in tileReferences)
                {
                    TileBase tile = tilemap.GetTile(position);
                    if (tile == tileData.tile && tileData.movable)
                    {
                        RuntimeTile newRuntimeTile = new RuntimeTile(tile, tileData.movementCost);
                        tileDictionary.Add(position, newRuntimeTile);
                        
                        if (spawnerGridPositions.TryGetValue(position, out SpawnPosition spawnerInstance))
                        {
                            newRuntimeTile.AddSpawnerInstance(spawnerInstance.gameObject);
                        }
                    }
                }
            }
        }

        public void UpdateUnitsPlacedInScenePosition()
        {
            foreach (SpawnerInstance spawner in spawnerInstances)
            {
                foreach (SpawnPosition spawnPosition in spawner.spawnPositions)
                {
                    Vector3Int cellPosition = tilemap.WorldToCell(spawnPosition.transform.position);
                    Debug.Log(cellPosition);
                    spawnPosition.transform.position = tilemap.GetCellCenterWorld(cellPosition);
                }
            }
        }
    }
}
