using System;
using System.Collections.Generic;
using System.Linq;
using Aoiti.Pathfinding;
using DataStructures.RuntimeSet;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Features.Tiles
{
    [CreateAssetMenu]
    public class TileRuntimeDictionary_SO : RuntimeDictionary_SO<Vector3Int, RuntimeTile>
    {
        private readonly Vector3Int[] _directions = new Vector3Int[4] {Vector3Int.left,Vector3Int.right,Vector3Int.up,Vector3Int.down };

        private Tilemap _tilemap;
        private Pathfinder<Vector3Int> _pathfinder;
        private Vector3Int _targetNode;
        
        public void Initialize(Tilemap tilemap, TileLookup[] tileReferences, Action<Dictionary<Vector3Int, RuntimeTile>> populateRuntimeSet)
        {
            _tilemap = tilemap;
            _pathfinder = new Pathfinder<Vector3Int>(DistanceFunc, ConnectionsAndCosts);
            populateRuntimeSet.Invoke(items);
        }

        public bool GenerateAStarPath(Vector3Int startNode, Vector3Int targetNode, out List<Vector3Int> path)
        {
            _targetNode = targetNode;
            return _pathfinder.GenerateAstarPath(startNode, targetNode, out path);
        }
        
        private float DistanceFunc(Vector3Int a, Vector3Int b)
        {
            return (a-b).sqrMagnitude;
        }

        private Dictionary<Vector3Int,float> ConnectionsAndCosts(Vector3Int a)
        {
            Dictionary<Vector3Int, float> result= new Dictionary<Vector3Int, float>();
            foreach (Vector3Int dir in _directions)
            {
                Vector3Int position = a + dir;
                if (!TryGetByGridPosition(position, out RuntimeTile runtimeTile)) continue;
                if (!runtimeTile.ContainsUnit || _targetNode == position) result.Add(position, runtimeTile.movementCost);
            }
            return result;
        }

        public Vector3 GetCellToWorldPosition(Vector3Int gridPosition)
        {
            return _tilemap.GetCellCenterWorld(gridPosition);
        }

        public Vector3Int GetWorldToCellPosition(Vector3 worldPosition)
        {
            return _tilemap.WorldToCell(worldPosition);
        }
        
        public void ClearContainedUnits()
        {
            foreach (var item in items)
            {
                item.Value.RemoveUnit();
            }
        }
        
        private Dictionary<Vector3Int, RuntimeTile> GetPlaceableTileBehaviours()
        {
            Dictionary<Vector3Int, RuntimeTile> placeableDictionary = new Dictionary<Vector3Int, RuntimeTile>();

            foreach (var item in items)
            {
                if (!item.Value.ContainsUnit)
                {
                    placeableDictionary.Add(item.Key, item.Value);
                }
            }

            return placeableDictionary;
        }

        public bool TryGetRandomPlaceableTileBehaviour(out KeyValuePair<Vector3Int, RuntimeTile> tileKeyValuePair)
        {
            Dictionary<Vector3Int, RuntimeTile> placeableDictionary = GetPlaceableTileBehaviours();
            
            if (placeableDictionary.Count == 0)
            {
                tileKeyValuePair = default;
                return false;
            }

            int randomElement = Random.Range(0, placeableDictionary.Count);
            tileKeyValuePair = placeableDictionary.ElementAt(randomElement);
            return true;
        }

        public bool ContainsGridPosition(Vector3Int gridPosition)
        {
            return items.ContainsKey(gridPosition);
        }

        public bool TryGetByGridPosition(Vector3Int gridPosition, out RuntimeTile runtimeTile)
        {
            return items.TryGetValue(gridPosition, out runtimeTile);
        }
    }
}
