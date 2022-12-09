using DataStructures.StateLogic;

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
            if (_battleBehaviour.HasTarget && _battleBehaviour.TargetInRange)
            {
                _battleBehaviour.RequestAttackState();
            }
            else if (_battleBehaviour.HasTarget && !_battleBehaviour.TargetInRange)
            {
                _battleBehaviour.RequestMovementState();
            }
        }

        public void Exit()
        {
        }
    }
}
