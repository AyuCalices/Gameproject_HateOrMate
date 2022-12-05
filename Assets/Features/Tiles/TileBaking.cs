using System;
using System.Collections.Generic;
using Features.Unit.GridMovement;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Features.Tiles
{
    public class TileBaking : MonoBehaviour
    {
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private TilemapRenderer tilemapRenderer;
        [SerializeField] private List<TileContainer> tileDataList;
        [SerializeField] private TileRuntimeDictionary_SO tileRuntimeDictionary;

        private void Awake()
        {
            CustomTypesUnity.Register();
            tileRuntimeDictionary.ClearContainedUnits();
            Bake();
            tileRuntimeDictionary.TilemapRenderer = tilemapRenderer;
            tileRuntimeDictionary.Tilemap = tilemap;
        }

        public void Bake()
        {
            foreach (Vector3Int position in tilemap.cellBounds.allPositionsWithin)
            {
                foreach (TileContainer tileData in tileDataList)
                {
                    TileBase tile = tilemap.GetTile(position);
                    if (tile == tileData.tile && tileData.movable)
                    {
                        tileRuntimeDictionary.Add(position, new TileContainer(tile, tileData.movable));
                    }
                }
            }
        }
    }

    [Serializable]
    public class TileContainer
    {
        public TileBase tile;
        public bool movable;
        
        public bool ContainsUnit => _containedUnitTilePlacementBehaviour != null;

        private NetworkedUnitTilePlacementBehaviour _containedUnitTilePlacementBehaviour;

        public TileContainer(TileBase tile, bool movable)
        {
            this.tile = tile;
            this.movable = movable;
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
