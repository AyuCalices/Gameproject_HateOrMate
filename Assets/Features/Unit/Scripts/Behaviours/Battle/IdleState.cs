using DataStructures.StateLogic;

namespace Features.Unit.Scripts.Behaviours.Battle
{
    public class IdleState : IState
    {
        private readonly NetworkedBattleBehaviour _battleBehaviour;

        public IdleState(NetworkedBattleBehaviour battleBehaviour)
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
