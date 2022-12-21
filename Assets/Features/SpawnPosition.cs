using Features.Battle.Scripts;
using Features.Tiles;
using UnityEngine;

namespace Features
{
    public class SpawnPosition : MonoBehaviour
    {
        [SerializeField] private BattleData_SO battleData;

        private RuntimeTile _runtimeTile;
        public RuntimeTile SpawnerInstanceTile => _runtimeTile;
        public Vector3Int GridPosition { get; private set; }
        public bool HasValidTile { get; private set; }
    
        private void Awake()
        {
            GridPosition = battleData.TileRuntimeDictionary.GetWorldToCellPosition(transform.position);
            HasValidTile = battleData.TileRuntimeDictionary.TryGetByGridPosition(GridPosition,
                out _runtimeTile);
        }
    }
}
