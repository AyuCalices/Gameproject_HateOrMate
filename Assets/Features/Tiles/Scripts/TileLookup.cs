using System;
using UnityEngine.Tilemaps;

namespace Features.Tiles.Scripts
{
    [Serializable]
    public struct TileLookup
    {
        public TileBase tile;
        public bool movable;
        public float movementCost;
    }
}
