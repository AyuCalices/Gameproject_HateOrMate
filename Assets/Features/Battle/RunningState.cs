using DataStructures.StateLogic;
using Photon.Pun;

namespace Features.Battle
{
    public class RunningState : IState
    {
        private readonly BattleManager _battleManager;
        
        public RunningState(BattleManager battleManager)
        {
            _battleManager = battleManager;
        }
    
        public void Enter()
        {
            InitializeStage();
        }

        public void Execute()
        {
            
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
