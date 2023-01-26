using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Behaviours.Stat;
using Features.Unit.Scripts.View;
using UnityEngine;

namespace Features.Unit.Scripts.Class
{
    public class TroopBattleClass : BattleClass
    {
        private float _attackSpeedDeltaTime;
        
        public TroopBattleClass(NetworkedStatsBehaviour ownerNetworkingStatsBehaviour,
            BattleBehaviour ownerBattleBehaviour,
            UnitBattleView ownerUnitBattleView) : base(ownerNetworkingStatsBehaviour, ownerBattleBehaviour, ownerUnitBattleView)
        {
            _attackSpeedDeltaTime = ownerNetworkingStatsBehaviour.NetworkedStatServiceLocator.GetTotalValue_CheckMin(StatType.Speed);
        }

        protected override void InternalInitializeBattleActions()
        {
            _attackSpeedDeltaTime = ownerNetworkingStatsBehaviour.NetworkedStatServiceLocator.GetTotalValue_CheckMin(StatType.Speed);
            ownerUnitBattleView.SetStaminaSlider(_attackSpeedDeltaTime, ownerNetworkingStatsBehaviour.NetworkedStatServiceLocator.GetTotalValue_CheckMin(StatType.Speed));
        }

        protected override void InternalUpdateBattleActions()
        {
            _attackSpeedDeltaTime -= Time.deltaTime;
            
            if (_attackSpeedDeltaTime <= 0)
            {
                _attackSpeedDeltaTime = ownerNetworkingStatsBehaviour.NetworkedStatServiceLocator.GetTotalValue_CheckMin(StatType.Speed);
                InternalOnPerformAction();
            }
            
            ownerUnitBattleView.SetStaminaSlider(_attackSpeedDeltaTime, ownerNetworkingStatsBehaviour.NetworkedStatServiceLocator.GetTotalValue_CheckMin(StatType.Speed));
        }

        protected override void InternalOnPerformAction()
        {
            SendAttack(ownerBattleBehaviour.GetTarget.Key, ownerNetworkingStatsBehaviour.NetworkedStatServiceLocator.GetTotalValue_CheckMin(StatType.Damage));
        }
        
        public override void OnStageEnd()
        {
            ownerUnitBattleView.ResetStaminaSlider();
        }
    }
}
