using DataStructures.StateLogic;
using Features.Unit.Scripts.Behaviours.Services.BattleBehaviour;

namespace Features.Unit.Scripts.Behaviours.States
{
    public class IdleState : IState
    {
        private readonly UnitBattleBehaviour _battleBehaviour;

        public IdleState(UnitBattleBehaviour battleBehaviour)
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
                _battleBehaviour.TryRequestMovementStateByClosestUnit();
            }
        }

        public void Exit()
        {
        }
    }
}
