using DataStructures.StateLogic;

namespace Features.Battle
{
    public class PausedState : IState
    {
        private BattleManager _battleManager;
        
        public PausedState(BattleManager battleManager)
        {
            _battleManager = battleManager;
        }
    
        public void Enter()
        {
        }

        public void Execute()
        {
        }

        public void Exit()
        {
        }
    }
}
