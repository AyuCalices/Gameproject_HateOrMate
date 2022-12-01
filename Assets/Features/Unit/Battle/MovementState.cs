using DataStructures.StateLogic;
using Features.GlobalReferences;
using Features.Unit.Modding;

namespace Features.Unit.Battle
{
    public class MovementState : IState
    {
        private readonly BattleActions _battleActions;
        
        public MovementState(BattleActions battleActions)
        {
            _battleActions = battleActions;
        }
    
        public void Enter()
        {
        }

        public void Execute()
        {
            _battleActions.Move();
        }

        public void Exit()
        {
        }
    }
}
