using Features.BattleScene.Scripts;
using Features.BattleScene.Scripts.StateMachine;
using Features.BattleScene.Scripts.States;
using Features.Unit.Scripts.Behaviours.States;
using UnityEngine;

namespace Features.Unit.Scripts.Behaviours.Services.BattleBehaviour
{
    public class PassiveBattleBehaviour : IBattleBehaviour
    {
        private readonly BattleData_SO _battleData;
        private readonly UnitServiceProvider _unitServiceProvider;
        private readonly UnitBattleBehaviour _unitBattleBehaviour;
    
        
        public PassiveBattleBehaviour(BattleData_SO battleData, UnitServiceProvider unitServiceProvider)
        {
            _battleData = battleData;
            _unitServiceProvider = unitServiceProvider;
            _unitBattleBehaviour = _unitServiceProvider.GetService<UnitBattleBehaviour>();
        }

        public void OnStageEnd()
        {
            if (_unitBattleBehaviour.CurrentState is DeathState)
            {
                ForceIdleState();
            }
        }

        public void Update() { }

        public void ForceIdleState()
        {
            _unitBattleBehaviour.StateMachine.ChangeState(new IdleState(_unitServiceProvider.GetService<UnitBattleBehaviour>()));
        }

        public void ForceBenchedState()
        {
            _unitBattleBehaviour.StateMachine.ChangeState(new BenchedState(_unitServiceProvider.GetService<UnitBattleBehaviour>()));
        }

        public bool TryRequestIdleState()
        {
            return false;
        }

        public bool TryRequestAttackState()
        {
            return false;
        }

        public bool TryRequestMovementStateByClosestUnit()
        {
            return false;
        }

        public bool TryRequestDeathState()
        {
            bool result = _battleData.StateIsValid(typeof(BattleState), StateProgressType.Execute);
            
            if (result)
            {
                _unitBattleBehaviour.StateMachine.ChangeState(new DeathState(_unitServiceProvider.GetService<UnitBattleBehaviour>()));
            }
            else
            {
                Debug.LogWarning("Requesting Death is only possible during Battle!");
            }

            return false;
        }
    }
}
