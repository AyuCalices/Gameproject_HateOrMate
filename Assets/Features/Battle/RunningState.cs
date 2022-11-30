using DataStructures.StateLogic;

namespace Features.Battle
{
    public class RunningState : IState
    {
        private BattleManager_SO _battleManager;
        
        public RunningState(BattleManager_SO battleManager)
        {
            _battleManager = battleManager;
        }
    
        public void Enter()
        {
            InitializeStage();
        }

        public void Execute()
        {
            throw new System.NotImplementedException();
        }

        public void Exit()
        {
            ResetStage();
        }

        private void InitializeStage()
        {
            
        }

        private void ResetStage()
        {
            
        }
    }
}
