using DataStructures.StateLogic;

namespace Features.Battle.Scripts
{
    public class BattleState : IState
    {
        private readonly BattleManager _battleManager;
        
        public BattleState(BattleManager battleManager)
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
