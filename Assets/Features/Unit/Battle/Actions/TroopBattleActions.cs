using Features.GlobalReferences;
using Features.Unit.Modding;
using Features.Unit.Modding.Stat;
using UnityEngine;

namespace Features.Unit.Battle
{
    public class TroopBattleActions : BattleActions
    {
        private float _attackSpeedDeltaTime;
        
        public TroopBattleActions(NetworkedUnitBehaviour ownerNetworkingUnitBehaviour,
            BattleBehaviour ownerBattleBehaviour,
            UnitView ownerUnitView, NetworkedUnitRuntimeSet_SO opponentNetworkedUnitRuntimeSet) : base(
            ownerNetworkingUnitBehaviour, ownerBattleBehaviour, ownerUnitView,
            opponentNetworkedUnitRuntimeSet)
        {
            _attackSpeedDeltaTime = ownerNetworkingUnitBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Speed);
        }
        
        protected override void InternalUpdateBattleActions()
        {
            _attackSpeedDeltaTime -= Time.deltaTime;
            
            if (_attackSpeedDeltaTime <= 0)
            {
                _attackSpeedDeltaTime = ownerNetworkingUnitBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Speed);
                InternalOnPerformAction();
            }
            
            ownerUnitView.SetStaminaSlider(_attackSpeedDeltaTime, ownerNetworkingUnitBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Speed));
        }

        protected override void InternalOnPerformAction()
        {
            PerformAttack(ownerNetworkingUnitBehaviour.ControlType);
        }
    }
}
