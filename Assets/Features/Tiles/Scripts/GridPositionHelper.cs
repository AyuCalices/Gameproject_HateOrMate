using Features.Battle.Scripts;
using Features.Unit.Scripts.Behaviours.Battle;
using UnityEngine;

namespace Features.Tiles.Scripts
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
