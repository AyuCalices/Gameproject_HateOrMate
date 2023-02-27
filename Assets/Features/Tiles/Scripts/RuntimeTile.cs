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
        public bool ContainsSpawner => _containedSpawnerInstance != null;
        public bool IsPlaceable => !ContainsSpawner && !ContainsUnit;

        private GameObject _containedUnitTilePlacementBehaviour;
        private GameObject _containedSpawnerInstance;

        public RuntimeTile(TileBase tile, float movementCost)
        {
            this.tile = tile;
            this.movementCost = movementCost;
        }
        
        public void AddSpawnerInstance(GameObject spawnerInstance)
        {
            if (ContainsSpawner)
            {
                Debug.LogWarning("Unit Has been overwritten!");
            }
            
            _containedSpawnerInstance = spawnerInstance;
        }

        public void RemoveSpawnerInstance()
        {
            if (!ContainsSpawner)
            {
                Debug.LogWarning("No Unit to be Removed!");
                return;
            }

            _containedSpawnerInstance = null;
        }

        public void AddUnit(GameObject localUnitTilePlacementBehaviour)
        {
            Debug.Log("o/");
            
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
