using System.Collections.Generic;
using System.Linq;
using Features.GlobalReferences;
using UnityEngine;

namespace Features.Unit.GridMovement
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private int width, height;
        [SerializeField] private GridRuntimeDictionary_SO gridRuntimeDictionary;
        [SerializeField] private TileBehaviour tilePrefab;

        private void Start()
        {
            GenerateGrid();
        }

        private void GenerateGrid()
        {
            gridRuntimeDictionary.Restore();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    TileBehaviour spawnedTile = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity, transform);
                    spawnedTile.name = $"Tile {x} {y}";

                    spawnedTile.transform.position = new Vector2(x - (width / 2f) + 0.5f, y - (height / 2f) + 0.5f);

                    bool isOffset = x % 2 == 0 && y % 2 != 0 || x % 2 != 0 && y % 2 == 0;
                    Vector2 gridPosition = new Vector2(x, y);
                    spawnedTile.Init(isOffset, gridPosition);
                
                    gridRuntimeDictionary.Add(gridPosition, spawnedTile);
                }
            }
        }
    }
}