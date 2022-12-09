using DataStructures.StateLogic;
using Features.Tiles;
using Features.Unit.GridMovement;
using Features.Unit.Modding;
using UnityEngine;

namespace Features.Unit.Battle.Scripts
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
            
            NetworkedUnitBehaviour closestUnit = _battleBehaviour.GetTarget.Key;
            Vector3Int enemyPosition = _tileRuntimeDictionary.GetWorldToCellPosition(closestUnit.transform.position);
            _ownerTilePlacementBehaviour.RequestMove(enemyPosition, 2);
        }

        public void Execute()
        {
            if (!_battleBehaviour.HasTarget)
            {
                _battleBehaviour.RequestIdleState();
            }
            else if (_battleBehaviour.HasTarget && _battleBehaviour.TargetInRange)
            {
                _battleBehaviour.RequestAttackState();
            }
        }

        public void Exit()
        {
        }
    }
}