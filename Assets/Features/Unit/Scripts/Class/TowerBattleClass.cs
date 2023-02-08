using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.DamageAnimation;
using Features.Unit.Scripts.Stats;
using Features.Unit.Scripts.View;
using UnityEngine;

namespace Features.Unit.Scripts.Class
{
    public class TowerBattleClass : BattleClass
    {
        private readonly BaseDamageAnimationBehaviour _baseDamageAnimationPrefab;
        
        private float TotalStamina => ownerNetworkingStatsBehaviour.GetFinalStat(StatType.Stamina);
        private float _currentStamina;
        private float StaminaRefreshTime => ownerNetworkingStatsBehaviour.GetFinalStat(StatType.Speed);
        private float _staminaRefreshTimeDelta;

        public TowerBattleClass(NetworkedStatsBehaviour ownerNetworkingStatsBehaviour, BattleBehaviour ownerBattleBehaviour,
            UnitBattleView ownerUnitBattleView, BaseDamageAnimationBehaviour baseDamageAnimationPrefab) : 
            base(ownerNetworkingStatsBehaviour, ownerBattleBehaviour, ownerUnitBattleView)
        {
            _baseDamageAnimationPrefab = baseDamageAnimationPrefab;
        }

        protected override void InternalInitializeBattleActions()
        {
            _currentStamina = TotalStamina;
            _staminaRefreshTimeDelta = StaminaRefreshTime;
            ownerUnitBattleView.SetStaminaSlider(_currentStamina, TotalStamina);
        }

        protected override void InternalUpdateBattleActions()
        {
            _staminaRefreshTimeDelta -= Time.deltaTime;

            if (_staminaRefreshTimeDelta > 0) return;
        
            _staminaRefreshTimeDelta = StaminaRefreshTime;
            if (_currentStamina <= TotalStamina)
            {
                _currentStamina++;
                ownerUnitBattleView.SetStaminaSlider(_currentStamina, TotalStamina);
            }
        }

        protected override void InternalOnPerformAction()
        {
            if (ownerBattleBehaviour.CurrentState is not AttackState) return;
            
            if (_currentStamina <= 0) return;

            _currentStamina--;
            ownerUnitBattleView.SetStaminaSlider(_currentStamina, TotalStamina);

            NetworkedBattleBehaviour targetUnit = ownerBattleBehaviour.GetTarget.Key;
            _baseDamageAnimationPrefab.InstantiateDamageAnimation(
                ownerBattleBehaviour, targetUnit, () =>
                {
                    SendAttack(targetUnit, ownerNetworkingStatsBehaviour.GetFinalStat(StatType.Damage));
                });
        }
        
        public override void OnStageEnd()
        {
            ownerUnitBattleView.ResetStaminaSlider();

            BaseDamageAnimationBehaviour.DestroyAllByPrefabReference(_baseDamageAnimationPrefab, ownerBattleBehaviour.PhotonView);
        }
    }
}
