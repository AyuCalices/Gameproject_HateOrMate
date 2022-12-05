using System.Collections.Generic;
using System.Linq;
using DataStructures.RuntimeSet;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Features.Tiles
{
    [CreateAssetMenu]
    public class TileRuntimeDictionary_SO : RuntimeDictionary_SO<Vector3Int, TileContainer>
    {
        public TilemapRenderer TilemapRenderer { get; set; }
        public Tilemap Tilemap { get; set; }

        public Vector3 GetCellToWorldPosition(Vector3Int gridPosition)
        {
            return Tilemap.GetCellCenterWorld(gridPosition);
        }

        public Vector3Int GetWorldToCellPosition(Vector3 worldPosition)
        {
            return Tilemap.WorldToCell(worldPosition);
        }
        
        public void ClearContainedUnits()
        {
            foreach (var item in items)
            {
                item.Value.RemoveUnit();
            }
        }
        
        private Dictionary<Vector3Int, TileContainer> GetPlaceableTileBehaviours()
        {
            Dictionary<Vector3Int, TileContainer> placeableDictionary = new Dictionary<Vector3Int, TileContainer>();

            foreach (var item in items)
            {
                if (!item.Value.ContainsUnit)
                {
                    placeableDictionary.Add(item.Key, item.Value);
                }
            }

            return placeableDictionary;
        }

        public bool TryGetRandomPlaceableTileBehaviour(out KeyValuePair<Vector3Int, TileContainer> tileKeyValuePair)
        {
            Dictionary<Vector3Int, TileContainer> placeableDictionary = GetPlaceableTileBehaviours();
            
            if (placeableDictionary.Count == 0)
            {
                tileKeyValuePair = default;
                return false;
            }

            int randomElement = Random.Range(0, placeableDictionary.Count);
            tileKeyValuePair = placeableDictionary.ElementAt(randomElement);
            return true;
        }
        
        /// <summary>
        /// When dragging an Object the tiles needs to be in front of the clicker UI element else moving position not possible
        /// </summary>
        /// <param name="orderInLayer"></param>
        /// <param name="highlightOrder"></param>
        public void SetAllOrderInLayer(int orderInLayer)
        {
            foreach (var item in items)
            {
                TilemapRenderer.sortingOrder = orderInLayer;
            }
        }

        public bool GetByGridPosition(Vector3Int gridPosition, out TileContainer tileContainer)
        {
            return items.TryGetValue(gridPosition, out tileContainer);
        }
    }
}
