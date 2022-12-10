using DataStructures.StateLogic;
using UnityEngine;

namespace Features.Unit.Battle.Scripts
{
    public class IdleState : IState
    {
        private readonly BattleBehaviour _battleBehaviour;

        public IdleState(BattleBehaviour battleBehaviour)
        {
            _battleBehaviour = battleBehaviour;
        }

        public void Enter()
        {
        }

        public void Execute()
        {
            if (!_battleBehaviour.TryRequestAttackState())
            {
                _battleBehaviour.TryRequestMovementStateByAI();
            }
        }

        public void Exit()
        {
        }
    }
}
