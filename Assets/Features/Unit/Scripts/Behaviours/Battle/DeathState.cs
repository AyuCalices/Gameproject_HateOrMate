using DataStructures.StateLogic;

namespace Features.Unit.Scripts.Behaviours.Battle
{
    public class DeathState : IState
    {
        private readonly NetworkedBattleBehaviour _battleBehaviour;
        
        public DeathState(NetworkedBattleBehaviour battleBehaviour)
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
