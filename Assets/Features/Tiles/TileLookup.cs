using System;
using UnityEngine.Tilemaps;

namespace Features.Tiles
{
    [Serializable]
    public struct TileLookup
    {
        public TileBase tile;
        public bool movable;
        public float movementCost;
    }
}
