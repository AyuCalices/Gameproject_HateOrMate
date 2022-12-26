using System;
using DataStructures.StateLogic;
using Features.Battle.Scripts;
using Features.Tiles;
using UnityEngine;

namespace Features.Unit.Battle.Scripts
{
    public class MovementState : IState
    {
        public static Action<BattleBehaviour, Vector3Int, int> onPerformMovement;
        
        private readonly BattleBehaviour _battleBehaviour;
        private readonly Vector3Int _targetPosition;
        private readonly int _skipLastMovementsCount;
        private readonly TileRuntimeDictionary_SO _tileRuntimeDictionary;

        public MovementState(BattleBehaviour battleBehaviour, Vector3Int targetPosition, int skipLastMovementsCount, TileRuntimeDictionary_SO tileRuntimeDictionary)
        {
            _battleBehaviour = battleBehaviour;
            _targetPosition = targetPosition;
            _skipLastMovementsCount = skipLastMovementsCount;
            _tileRuntimeDictionary = tileRuntimeDictionary;
        }

        public void Enter()
        {
            Vector3Int originPosition = _tileRuntimeDictionary.GetWorldToCellPosition(_battleBehaviour.transform.position);
            if (_tileRuntimeDictionary.GenerateAStarPath(originPosition, _targetPosition, out _))
            {
                onPerformMovement.Invoke(_battleBehaviour, _targetPosition, _skipLastMovementsCount);
            }
            else
            {
                _battleBehaviour.ForceIdleState();
            }
        }

        public void Execute()
        {
        }

        public void Exit()
        {  
        }
    }
}
