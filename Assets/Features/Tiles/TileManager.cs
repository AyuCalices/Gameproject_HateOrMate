using System;
using System.Collections.Generic;
using Features.Unit.GridMovement;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Features.Tiles
{
    public class TileManager : MonoBehaviour
    {
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private TileLookup[] tileReferences;
        [SerializeField] private TileRuntimeDictionary_SO tileRuntimeDictionary;

        private void Awake()
        {
            tileRuntimeDictionary.ClearContainedUnits();
            tileRuntimeDictionary.Initialize(tilemap, tileReferences, PopulateTileRuntimeDictionary);
        }

        private void PopulateTileRuntimeDictionary(Dictionary<Vector3Int, RuntimeTile> tileDictionary)
        {
            foreach (Vector3Int position in tilemap.cellBounds.allPositionsWithin)
            {
                foreach (TileLookup tileData in tileReferences)
                {
                    TileBase tile = tilemap.GetTile(position);
                    if (tile == tileData.tile && tileData.movable)
                    {
                        tileDictionary.Add(position, new RuntimeTile(tile, tileData.movementCost));
                    }
                }
            }
        }
    }
}
