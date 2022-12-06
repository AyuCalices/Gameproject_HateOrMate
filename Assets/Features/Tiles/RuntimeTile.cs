using System;
using Features.Unit.GridMovement;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Features.Tiles
{
    [Serializable]
    public class RuntimeTile
    {
        public TileBase tile;
        public float movementCost;
        
        public bool ContainsUnit => _containedUnitTilePlacementBehaviour != null;

        private NetworkedUnitTilePlacementBehaviour _containedUnitTilePlacementBehaviour;

        public RuntimeTile(TileBase tile, float movementCost)
        {
            this.tile = tile;
            this.movementCost = movementCost;
        }
        
        public void AddUnit(NetworkedUnitTilePlacementBehaviour localUnitTilePlacementBehaviour)
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
