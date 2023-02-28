using Features.Battle.Scripts;
using Features.Battle.StateMachine;
using UnityEngine;

namespace Features.Unit.Scripts.Behaviours.Battle.BattleBehaviour
{
    public class PassiveBattleBehaviour : IBattleBehaviour
    {
        private readonly BattleData_SO _battleData;
        private readonly UnitServiceProvider _unitServiceProvider;
        private readonly NetworkedBattleBehaviour _networkedBattleBehaviour;
    
        
        public PassiveBattleBehaviour(BattleData_SO battleData, UnitServiceProvider unitServiceProvider)
        {
            _battleData = battleData;
            _unitServiceProvider = unitServiceProvider;
            _networkedBattleBehaviour = _unitServiceProvider.GetService<NetworkedBattleBehaviour>();
        }

        public void OnStageEnd()
        {
            if (_networkedBattleBehaviour.CurrentState is DeathState)
            {
                ForceIdleState();
            }
        }

        public void Update() { }

        public void ForceIdleState()
        {
            _networkedBattleBehaviour.StateMachine.ChangeState(new IdleState(_unitServiceProvider.GetService<NetworkedBattleBehaviour>()));
        }

        public void ForceBenchedState()
        {
            _networkedBattleBehaviour.StateMachine.ChangeState(new BenchedState(_unitServiceProvider.GetService<NetworkedBattleBehaviour>()));
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
                _networkedBattleBehaviour.StateMachine.ChangeState(new DeathState(_unitServiceProvider.GetService<NetworkedBattleBehaviour>()));
            }
            else
            {
                Debug.LogWarning("Requesting Death is only possible during Battle!");
            }

            return false;
        }
    }
}
