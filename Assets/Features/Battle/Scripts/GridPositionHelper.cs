using Features.Tiles;
using Features.Unit.Battle.Scripts;
using UnityEngine;

namespace Features.Battle.Scripts
{
    public static class GridPositionHelper
    {
        public static void UpdateUnitOnRuntimeTiles(BattleData_SO battleData, NetworkedBattleBehaviour battleBehaviour, Vector3Int currentCellPosition, Vector3Int nextCellPosition)
        {
            RuntimeTile targetTileContainer = battleData.TileRuntimeDictionary.GetContent(nextCellPosition);
            targetTileContainer.AddUnit(battleBehaviour.gameObject);

            if (battleData.TileRuntimeDictionary.TryGetContent(currentCellPosition, out RuntimeTile previousTileBehaviour))
            {
                previousTileBehaviour.RemoveUnit();
            }
        }
        
        public static Vector3Int GetCurrentCellPosition(BattleData_SO battleData, Transform currentTransform)
        {
            return battleData.TileRuntimeDictionary.GetWorldToCellPosition(currentTransform.position);
        }

        public static bool IsViablePosition(BattleData_SO battleData, Vector3Int targetCellPosition)
        {
            return battleData.TileRuntimeDictionary.TryGetByGridPosition(targetCellPosition, out RuntimeTile runtimeTile) 
                   && runtimeTile.IsPlaceable;
        }
    }
}
