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
        
        public TroopBattleActions(NetworkedStatsBehaviour ownerNetworkingStatsBehaviour,
            BattleBehaviour ownerBattleBehaviour,
            UnitView ownerUnitView) : base(ownerNetworkingStatsBehaviour, ownerBattleBehaviour, ownerUnitView)
        {
            _attackSpeedDeltaTime = ownerNetworkingStatsBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Speed);
        }

        protected override void InternalInitializeBattleActions()
        {
            _attackSpeedDeltaTime = ownerNetworkingStatsBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Speed);
            ownerUnitView.SetStaminaSlider(_attackSpeedDeltaTime, ownerNetworkingStatsBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Speed));
        }

        protected override void InternalUpdateBattleActions()
        {
            _attackSpeedDeltaTime -= Time.deltaTime;
            
            if (_attackSpeedDeltaTime <= 0)
            {
                _attackSpeedDeltaTime = ownerNetworkingStatsBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Speed);
                InternalOnPerformAction();
            }
            
            ownerUnitView.SetStaminaSlider(_attackSpeedDeltaTime, ownerNetworkingStatsBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Speed));
        }

        protected override void InternalOnPerformAction()
        {
            SendAttack(ownerBattleBehaviour.GetTarget.Key);
        }
        
        public override void OnStageEnd()
        {
            
        }
    }
}
