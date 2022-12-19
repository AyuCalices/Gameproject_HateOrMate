using System.Collections.Generic;
using Features.Battle.Scripts;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Features.Tiles
{
    public class TileManager : MonoBehaviour
    {
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private BattleData_SO battleData;
        [SerializeField] private TileLookup[] tileReferences;
        [SerializeField] private List<GameObject> spawnerInstances;

        private void Awake()
        {
            battleData.TileRuntimeDictionary.ClearContainedUnits();
            battleData.TileRuntimeDictionary.Initialize(tilemap, tileReferences, PopulateTileRuntimeDictionary);
        }

        private void PopulateTileRuntimeDictionary(Dictionary<Vector3Int, RuntimeTile> tileDictionary)
        {
            Dictionary<Vector3Int, GameObject> spawnerGridPositions = new Dictionary<Vector3Int, GameObject>();
            foreach (GameObject spawner in spawnerInstances)
            {
                spawnerGridPositions.Add(tilemap.WorldToCell(spawner.transform.position), spawner);
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
                        
                        if (spawnerGridPositions.TryGetValue(position, out GameObject spawnerInstance))
                        {
                            newRuntimeTile.AddSpawnerInstance(spawnerInstance);
                        }
                    }
                }
            }
        }

        public void UpdateUnitsPlacedInScenePosition()
        {
            foreach (GameObject unit in spawnerInstances)
            {
                Vector3Int cellPosition = tilemap.WorldToCell(unit.transform.position);
                unit.transform.position = tilemap.GetCellCenterWorld(cellPosition);
            }
        }
    }
}
