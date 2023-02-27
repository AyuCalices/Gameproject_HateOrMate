using Features.Battle.Scripts;
using Features.Battle.Scripts.Unit.ServiceLocatorSystem;
using Features.Loot.Scripts.ModView;
using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.DamageAnimation;
using Features.Unit.Scripts.Stats;
using Features.Unit.Scripts.View;
using Photon.Pun;
using UnityEngine;

namespace Features.Unit.Scripts.Class
{
    public class TowerBattleClass : BattleClass
    {
        private readonly BaseDamageAnimationBehaviour _baseDamageAnimationPrefab;
        
        private float TotalStamina => ownerUnitServiceProvider.GetService<NetworkedStatsBehaviour>().GetFinalStat(StatType.Stamina);
        private float _currentStamina;
        private float StaminaRefreshTime => ownerUnitServiceProvider.GetService<NetworkedStatsBehaviour>().GetFinalStat(StatType.Speed);
        private float _staminaRefreshTimeDelta;

        public TowerBattleClass(UnitServiceProvider ownerUnitServiceProvider, BaseDamageAnimationBehaviour baseDamageAnimationPrefab) : 
            base(ownerUnitServiceProvider)
        {
            _baseDamageAnimationPrefab = baseDamageAnimationPrefab;
        }

        protected override void InternalInitializeBattleActions()
        {
            _currentStamina = TotalStamina;
            _staminaRefreshTimeDelta = StaminaRefreshTime;
            ownerUnitServiceProvider.GetService<UnitBattleView>().SetStaminaSlider(_currentStamina, TotalStamina);
        }

        protected override void InternalUpdateBattleActions()
        {
            _staminaRefreshTimeDelta -= Time.deltaTime;

            if (_staminaRefreshTimeDelta > 0) return;
        
            _staminaRefreshTimeDelta = StaminaRefreshTime;
            if (_currentStamina <= TotalStamina)
            {
                _currentStamina++;
                ownerUnitServiceProvider.GetService<UnitBattleView>().SetStaminaSlider(_currentStamina, TotalStamina);
            }
        }

        protected override void InternalOnPerformAction()
        {
            if (ownerUnitServiceProvider.GetService<NetworkedBattleBehaviour>().CurrentState is not AttackState) return;
            
            if (_currentStamina <= 0) return;

            _currentStamina--;
            ownerUnitServiceProvider.GetService<UnitBattleView>().SetStaminaSlider(_currentStamina, TotalStamina);

            if (ownerUnitServiceProvider.GetService<NetworkedBattleBehaviour>().BattleBehaviour is not ActiveBattleBehaviour activeBattleBehaviour) 
            {
                Debug.LogWarning($"OnPerformAction failed, because this Unit {ownerUnitServiceProvider.name} doesnt have an ActiveBattleBehaviour!");
                return;
            }

            UnitServiceProvider targetUnit = activeBattleBehaviour.GetTarget.Key;
            _baseDamageAnimationPrefab.InstantiateDamageAnimation(
                ownerUnitServiceProvider, targetUnit, () =>
                {
                    SendAttack(targetUnit, ownerUnitServiceProvider.GetService<NetworkedStatsBehaviour>().GetFinalStat(StatType.Damage));
                });
        }
        
        public override void OnStageEnd()
        {
            ownerUnitServiceProvider.GetService<UnitBattleView>().ResetStaminaSlider();

            BaseDamageAnimationBehaviour.DestroyAllByPrefabReference(_baseDamageAnimationPrefab, ownerUnitServiceProvider.GetService<PhotonView>());
        }
    }
}
