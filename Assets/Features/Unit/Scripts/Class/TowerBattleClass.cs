using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Behaviours.Stat;
using Features.Unit.Scripts.DamageAnimation;
using Features.Unit.Scripts.View;
using UnityEngine;

namespace Features.Unit.Scripts.Class
{
    public class TowerBattleClass : BattleClass
    {
        private readonly BaseDamageAnimationBehaviour _baseDamageAnimationPrefab;
        private readonly float _towerDamageMultiplier;
        
        private float TotalStamina => ownerNetworkingStatsBehaviour.NetworkedStatServiceLocator.GetTotalValue_CheckMin(StatType.Stamina);
        private float _currentStamina;
        private float StaminaRefreshTime => ownerNetworkingStatsBehaviour.NetworkedStatServiceLocator.GetTotalValue_CheckMin(StatType.Speed);
        private float _staminaRefreshTimeDelta;

        public TowerBattleClass(NetworkedStatsBehaviour ownerNetworkingStatsBehaviour, BattleBehaviour ownerBattleBehaviour,
            UnitBattleView ownerUnitBattleView, BaseDamageAnimationBehaviour baseDamageAnimationPrefab, float towerDamageMultiplier) : 
            base(ownerNetworkingStatsBehaviour, ownerBattleBehaviour, ownerUnitBattleView)
        {
            _baseDamageAnimationPrefab = baseDamageAnimationPrefab;
            _towerDamageMultiplier = towerDamageMultiplier;
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
                    SendAttack(targetUnit, ownerNetworkingStatsBehaviour.NetworkedStatServiceLocator.GetTotalValue_CheckMin(StatType.Damage) * _towerDamageMultiplier);
                });
        }
        
        public override void OnStageEnd()
        {
            ownerUnitBattleView.ResetStaminaSlider();

            BaseDamageAnimationBehaviour.DestroyAllByPrefabReference(_baseDamageAnimationPrefab, ownerBattleBehaviour.PhotonView);
        }
    }
}
