using DataStructures.StateLogic;

namespace Features.Battle
{
    public class PausedState : IState
    {
        private BattleManager_SO _battleManager;
        
        public PausedState(BattleManager_SO battleManager)
        {
            _battleManager = battleManager;
        }
    
        public void Enter()
        {
            throw new System.NotImplementedException();
        }

        public void Execute()
        {
            throw new System.NotImplementedException();
        }

        public void Exit()
        {
            throw new System.NotImplementedException();
        }
    }
}
