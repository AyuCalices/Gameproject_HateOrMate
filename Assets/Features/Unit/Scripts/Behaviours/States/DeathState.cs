using System;
using DataStructures.StateLogic;
using Features.Unit.Scripts.Behaviours.Services.BattleBehaviour;

namespace Features.Unit.Scripts.Behaviours.States
{
    public class DeathState : IState
    {
        public static Action onUnitEnterDeathState;
        
        private readonly UnitBattleBehaviour _battleBehaviour;
        
        public DeathState(UnitBattleBehaviour battleBehaviour)
        {
            _battleBehaviour = battleBehaviour;
        }

        public void Enter()
        {
            onUnitEnterDeathState?.Invoke();
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
