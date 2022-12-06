using System.Collections.Generic;
using DataStructures.StateLogic;
using Features.Tiles;
using Features.Unit.GridMovement;
using Features.Unit.Modding;
using ThirdParty.LeanTween.Framework;
using UnityEngine;

namespace Features.Unit.Battle
{
    public class MovementState : IState
    {
        private readonly BattleBehaviour _battleBehaviour;
        private readonly TileRuntimeDictionary_SO _tileRuntimeDictionary;
        private readonly NetworkedUnitTilePlacementBehaviour _ownerTilePlacementBehaviour;
        
        
        public MovementState(BattleBehaviour battleBehaviour, TileRuntimeDictionary_SO tileRuntimeDictionary)
        {
            _battleBehaviour = battleBehaviour;
            _tileRuntimeDictionary = tileRuntimeDictionary;
            _ownerTilePlacementBehaviour = battleBehaviour.GetComponent<NetworkedUnitTilePlacementBehaviour>();
        }

        public void Enter()
        {
        }

        public void Execute()
        {
            if (LeanTween.isTweening(_battleBehaviour.gameObject)) return;
            
            if (!_battleBehaviour.TryGetTarget(out NetworkedUnitBehaviour closestUnit)) return;

            Vector3Int enemyPosition = _tileRuntimeDictionary.GetWorldToCellPosition(closestUnit.transform.position);

            if (_tileRuntimeDictionary.GenerateAStarPath(_ownerTilePlacementBehaviour.GridPosition,
                enemyPosition, out List<Vector3Int> path) && path.Count > 1)
            {
                _ownerTilePlacementBehaviour.RequestMove(path[0]);
            }
        }

        public void Exit()
        {
        }
    }
}
