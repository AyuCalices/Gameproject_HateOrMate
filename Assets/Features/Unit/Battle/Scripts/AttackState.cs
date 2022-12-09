using DataStructures.StateLogic;
using Features.Unit.Battle.Scripts.Actions;
using Features.Unit.Modding;

namespace Features.Unit.Battle.Scripts
{
    public class AttackState : IState
    {
        private readonly BattleBehaviour _battleBehaviour;
        private readonly BattleActions _battleActions;
        
        
        public AttackState(BattleBehaviour battleBehaviour, BattleActions battleActions)
        {
            _battleBehaviour = battleBehaviour;
            _battleActions = battleActions;
        }
    
        public void Enter()
        {
            _battleActions.InitializeBattleActions();
        }

        public void Execute()
        {
            if (!_battleBehaviour.HasTarget)
            {
                _battleBehaviour.RequestIdleState();
            }
            else if (_battleBehaviour.HasTarget && !_battleBehaviour.TargetInRange)
            {
                _battleBehaviour.RequestMovementState();
            }
            else
            {
                _battleActions.UpdateBattleActions();
            }
        }

        public void Exit()
        {
        }
    }
}
