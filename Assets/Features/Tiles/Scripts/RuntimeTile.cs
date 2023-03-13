using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Features.Tiles.Scripts
{
    [Serializable]
    public class RuntimeTile
    {
        public TileBase tile;
        public float movementCost;
        
        public bool ContainsUnit => _containedUnitTilePlacementBehaviour != null;
        public bool IsPlaceable => _containedSpawnerInstance == null && !ContainsUnit;

        private GameObject _containedUnitTilePlacementBehaviour;
        private GameObject _containedSpawnerInstance;

        public RuntimeTile(TileBase tile, float movementCost)
        {
            this.tile = tile;
            this.movementCost = movementCost;
        }
        
        public void AddSpawnerInstance(GameObject spawnerInstance)
        {
            if (_containedSpawnerInstance != null)
            {
                Debug.LogWarning("Unit Has been overwritten!");
            }
            
            _containedSpawnerInstance = spawnerInstance;
        }

        public void RemoveSpawnerInstance()
        {
            if (_containedSpawnerInstance == null)
            {
                Debug.LogWarning("No Unit to be Removed!");
                return;
            }

            _containedSpawnerInstance = null;
        }

        public void AddUnit(GameObject localUnitTilePlacementBehaviour)
        {
            if (ContainsUnit)
            {
                Debug.LogWarning("Unit Has been overwritten!");
            }
            
            _containedUnitTilePlacementBehaviour = localUnitTilePlacementBehaviour;
        }

        public void RemoveUnit()
        {
            if (!ContainsUnit)
            {
                Debug.LogWarning("No Unit to be Removed!");
                return;
            }

            _containedUnitTilePlacementBehaviour = null;
        }
    }
}
