using DataStructures.StateLogic;

namespace Features.Unit.Battle
{
    public class DeathState : IState
    {
        private BattleBehaviour _battleBehaviour;
        
        public DeathState(BattleBehaviour battleBehaviour)
        {
            _battleBehaviour = battleBehaviour;
        }

        public void Enter()
        {
            _battleBehaviour.gameObject.SetActive(false);
        }

        public void Execute()
        {
        }

        public void Exit()
        {
            _battleBehaviour.gameObject.SetActive(true);
        }
    }
}
