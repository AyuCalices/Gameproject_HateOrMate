using Features.GlobalReferences.Scripts;
using Features.Unit.Modding;
using Features.Unit.Modding.Stat;
using Features.Unit.View;
using UnityEngine;

namespace Features.Unit.Battle.Scripts.Actions
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

        protected override void InternalInitializeBattleActions()
        {
            _attackSpeedDeltaTime = ownerNetworkingUnitBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Speed);
            ownerUnitView.SetStaminaSlider(_attackSpeedDeltaTime, ownerNetworkingUnitBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Speed));
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
            NetworkedUnitBehaviour closestUnit = ownerBattleBehaviour.NetworkedUnitBehaviour;
            SendAttack(ownerNetworkingUnitBehaviour.ControlType, closestUnit);
        }
        
        public override void OnStageEnd()
        {
            
        }
    }
}