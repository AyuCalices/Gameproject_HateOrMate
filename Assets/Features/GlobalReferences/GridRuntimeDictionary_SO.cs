using System.Collections.Generic;
using System.Linq;
using DataStructures.RuntimeSet;
using Features.Unit.GridMovement;
using UnityEngine;

namespace Features.GlobalReferences
{
    [CreateAssetMenu]
    public class GridRuntimeDictionary_SO : RuntimeDictionary_SO<Vector2, TileBehaviour>
    {
        private Dictionary<Vector2, TileBehaviour> GetPlaceableTileBehaviours()
        {
            Dictionary<Vector2, TileBehaviour> placeableDictionary = new Dictionary<Vector2, TileBehaviour>();

            foreach (var item in items)
            {
                if (!item.Value.ContainsUnit)
                {
                    placeableDictionary.Add(item.Key, item.Value);
                }
            }

            return placeableDictionary;
        }

        public bool TryGetRandomPlaceableTileBehaviour(out KeyValuePair<Vector2, TileBehaviour> tileKeyValuePair)
        {
            Dictionary<Vector2, TileBehaviour> placeableDictionary = GetPlaceableTileBehaviours();
            
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
        public void SetAllOrderInLayer(int orderInLayer, int highlightOrder)
        {
            foreach (var item in items)
            {
                item.Value.SetOrderInLayer(orderInLayer, highlightOrder);
            }
        }

        public bool GetByGridPosition(Vector2 gridPosition, out TileBehaviour tileBehaviour)
        {
            return items.TryGetValue(gridPosition, out tileBehaviour);
        }
    }
}
