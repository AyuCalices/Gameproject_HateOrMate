using DataStructures.StateLogic;
using UnityEngine;

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
            Debug.Log("enter");
            _battleBehaviour.gameObject.SetActive(false);
        }

        public void Execute()
        {
        }

        public void Exit()
        {
            Debug.Log("exit");
            _battleBehaviour.gameObject.SetActive(true);
        }
    }
}
