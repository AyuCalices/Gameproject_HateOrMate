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
        [SerializeField] private Transform cam;

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

                    bool isOffset = x % 2 == 0 && y % 2 != 0 || x % 2 != 0 && y % 2 == 0;
                    Vector2 gridPosition = new Vector2(x, y);
                    spawnedTile.Init(isOffset, gridPosition);
                
                    gridRuntimeDictionary.Add(gridPosition, spawnedTile);
                }
            }
            
            cam.transform.position = new Vector3((float) width / 2 - 0.5f, (float) height / 2 - 0.5f, -10);
        }

        public void AddUnitToRandom(NetworkedUnitTilePlacementBehaviour networkedUnitBehaviour)
        {
            int randomElement = Random.Range(0, gridRuntimeDictionary.GetItems().Count);
            KeyValuePair<Vector2, TileBehaviour> keyValuePair = gridRuntimeDictionary.GetItems().ElementAt(randomElement);
            if (keyValuePair.Value.ContainsUnit)
            {
                keyValuePair.Value.AddUnit(networkedUnitBehaviour);
                networkedUnitBehaviour.transform.position = keyValuePair.Value.transform.position;
                networkedUnitBehaviour.GridPosition = keyValuePair.Key;
            }
        }
    }
}