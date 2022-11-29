using System.Collections.Generic;
using System.Linq;
using Features.Unit;
using UnityEngine;

namespace Features.Grid
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private int _width, _height;

        [SerializeField] private Tile _tilePrefab;

        [SerializeField] private Transform _cam;

        private Dictionary<Vector2, Tile> _tiles;

        private void Start()
        {
            GenerateGrid();
        }

        private void GenerateGrid()
        {
            _tiles = new Dictionary<Vector2, Tile>();
            for (int x = 0; x < _width; x++)
            for (int y = 0; y < _height; y++)
            {
                Tile spawnedTile = Instantiate(_tilePrefab, new Vector3(x, y), Quaternion.identity, transform);
                spawnedTile.name = $"Tile {x} {y}";

                bool isOffset = x % 2 == 0 && y % 2 != 0 || x % 2 != 0 && y % 2 == 0;
                spawnedTile.Init(isOffset);


                _tiles[new Vector2(x, y)] = spawnedTile;
            }

            _cam.transform.position = new Vector3((float) _width / 2 - 0.5f, (float) _height / 2 - 0.5f, -10);
        }

        public Tile GetTileAtPosition(Vector2 pos)
        {
            if (_tiles.TryGetValue(pos, out Tile tile)) return tile;
            return null;
        }

        public void AddUnitToRandom(LocalUnitBehaviour localUnitBehaviour)
        {
            int randomElement = Random.Range(0, _tiles.Count);
            KeyValuePair<Vector2, Tile> keyValuePair = _tiles.ElementAt(randomElement);
            if (keyValuePair.Value.HasUnit)
            {
                keyValuePair.Value.AddUnit(localUnitBehaviour);
                localUnitBehaviour.transform.position = keyValuePair.Value.transform.position;
            }
        }
    }
}