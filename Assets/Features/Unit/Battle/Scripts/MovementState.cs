using System;
using DataStructures.StateLogic;
using Features.Battle.Scripts;
using UnityEngine;

namespace Features.Unit.Battle.Scripts
{
    public class MovementState : IState
    {
        public static Func<BattleBehaviour, Vector3Int, int, bool> onPerformMovement;
        
        private readonly BattleBehaviour _battleBehaviour;
        private readonly Vector3Int _targetPosition;
        private readonly int _skipLastMovementsCount;

        public MovementState(BattleBehaviour battleBehaviour, Vector3Int targetPosition, int skipLastMovementsCount)
        {
            _battleBehaviour = battleBehaviour;
            _targetPosition = targetPosition;
            _skipLastMovementsCount = skipLastMovementsCount;
        }

        public void Enter()
        {
            if (!onPerformMovement.Invoke(_battleBehaviour, _targetPosition, _skipLastMovementsCount))
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
