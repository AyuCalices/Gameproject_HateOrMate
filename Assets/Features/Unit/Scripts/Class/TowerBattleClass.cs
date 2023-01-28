using System.Collections.Generic;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Behaviours.Stat;
using Features.Unit.Scripts.View;
using UnityEngine;

namespace Features.Unit.Scripts.Class
{
    public class TowerBattleClass : BattleClass
    {
        private readonly DamageProjectileBehaviour _damageProjectilePrefab;
        private readonly float _towerDamageMultiplier;
        private readonly List<DamageProjectileBehaviour> _instantiatedProjectiles;
        
        private float TotalStamina => ownerNetworkingStatsBehaviour.NetworkedStatServiceLocator.GetTotalValue_CheckMin(StatType.Stamina);
        private float _currentStamina;
        private float StaminaRefreshTime => ownerNetworkingStatsBehaviour.NetworkedStatServiceLocator.GetTotalValue_CheckMin(StatType.Speed);
        private float _staminaRefreshTimeDelta;

        public TowerBattleClass(NetworkedStatsBehaviour ownerNetworkingStatsBehaviour, BattleBehaviour ownerBattleBehaviour,
            UnitBattleView ownerUnitBattleView, DamageProjectileBehaviour damageProjectilePrefab, float towerDamageMultiplier) : 
            base(ownerNetworkingStatsBehaviour, ownerBattleBehaviour, ownerUnitBattleView)
        {
            _instantiatedProjectiles = new List<DamageProjectileBehaviour>();
            _damageProjectilePrefab = damageProjectilePrefab;
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

            NetworkedBattleBehaviour closestStats = ownerBattleBehaviour.GetTarget.Key;
            DamageProjectileBehaviour instantiatedProjectile = _damageProjectilePrefab.FireProjectile(
                ownerBattleBehaviour.transform.position,
                closestStats.transform.position, closestStats.PhotonView.ViewID);
            
            _instantiatedProjectiles.Add(instantiatedProjectile);
            instantiatedProjectile.RegisterOnCompleteAction(() =>
            {
                SendAttack(closestStats, ownerNetworkingStatsBehaviour.NetworkedStatServiceLocator.GetTotalValue_CheckMin(StatType.Damage) * _towerDamageMultiplier);
                _instantiatedProjectiles.Remove(instantiatedProjectile);
            });
        }
        
        public override void OnStageEnd()
        {
            ownerUnitBattleView.ResetStaminaSlider();
            
            foreach (DamageProjectileBehaviour instantiatedProjectile in _instantiatedProjectiles)
            {
                instantiatedProjectile.CancelProjectile();
            }
                
            _instantiatedProjectiles.Clear();
        }
    }
}
