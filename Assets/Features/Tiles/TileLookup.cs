using System;
using UnityEngine.Tilemaps;

[Serializable]
public struct TileLookup
{
    public TileBase tile;
    public bool movable;
    public float movementCost;
}
