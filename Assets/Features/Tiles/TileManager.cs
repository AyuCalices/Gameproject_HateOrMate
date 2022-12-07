using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Features.Tiles
{
    public class TileManager : MonoBehaviour
    {
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private TileRuntimeDictionary_SO tileRuntimeDictionary;
        [SerializeField] private TileLookup[] tileReferences;
        [SerializeField] private List<GameObject> unitsPlacedInScene;

        private void Awake()
        {
            tileRuntimeDictionary.ClearContainedUnits();
            tileRuntimeDictionary.Initialize(tilemap, tileReferences, PopulateTileRuntimeDictionary);
        }

        private void PopulateTileRuntimeDictionary(Dictionary<Vector3Int, RuntimeTile> tileDictionary)
        {
            Dictionary<Vector3Int, GameObject> gridPositions = new Dictionary<Vector3Int, GameObject>();
            foreach (GameObject unit in unitsPlacedInScene)
            {
                gridPositions.Add(tilemap.WorldToCell(unit.transform.position), unit);
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
                        
                        if (gridPositions.TryGetValue(position, out GameObject unit))
                        {
                            newRuntimeTile.AddUnit(unit);
                            Debug.Log("o/");
                        }
                    }
                }
            }
        }

        public void UpdateUnitsPlacedInScenePosition()
        {
            foreach (GameObject unit in unitsPlacedInScene)
            {
                Vector3Int cellPosition = tilemap.WorldToCell(unit.transform.position);
                unit.transform.position = tilemap.GetCellCenterWorld(cellPosition);
            }
        }
    }
}
