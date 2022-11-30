using DataStructures.StateLogic;
using Features.GlobalReferences;
using Features.Unit.Modding;
using Features.Unit.Modding.Stat;
using UnityEngine;

namespace Features.Unit.Battle
{
    public class AttackState : IState
    {
        private float _attackDeltaTime;
        private readonly NetworkedUnitBehaviour _ownerNetworkedUnitBehaviour;
        private readonly NetworkedUnitRuntimeSet_SO _opponentNetworkedUnitRuntimeSet;
        private readonly BattleActions_SO _battleActions;
        
        public AttackState(NetworkedUnitBehaviour ownerNetworkedUnitBehaviour, NetworkedUnitRuntimeSet_SO opponentNetworkedUnitRuntimeSet, BattleActions_SO battleActions)
        {
            _ownerNetworkedUnitBehaviour = ownerNetworkedUnitBehaviour;
            _opponentNetworkedUnitRuntimeSet = opponentNetworkedUnitRuntimeSet;
            _battleActions = battleActions;
        }
    
        public void Enter()
        {
            _attackDeltaTime = _ownerNetworkedUnitBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Speed);
        }

        public void Execute()
        {
            //TODO: send event for attackTime UI updated
            
            _attackDeltaTime -= Time.deltaTime;
            if (_attackDeltaTime <= 0)
            {
                _battleActions.OnCastComplete(_ownerNetworkedUnitBehaviour, _opponentNetworkedUnitRuntimeSet);
                _attackDeltaTime = _ownerNetworkedUnitBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Speed);
            }

            _battleActions.UpdateBattleActions(_ownerNetworkedUnitBehaviour);
        }

        public void Exit()
        {
        }
    }
}
