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
        private readonly List<DamageProjectileBehaviour> _instantiatedProjectiles;
        private readonly int _totalStamina;
        private int _currentStamina;
        private readonly float _staminaRefreshTime;
        private float _staminaRefreshTimeDelta;

        public TowerBattleClass(NetworkedStatsBehaviour ownerNetworkingStatsBehaviour, BattleBehaviour ownerBattleBehaviour,
            UnitBattleView ownerUnitBattleView, DamageProjectileBehaviour damageProjectilePrefab, int totalStamina,
            float staminaRefreshTime) : base(ownerNetworkingStatsBehaviour, ownerBattleBehaviour, ownerUnitBattleView)
        {
            _instantiatedProjectiles = new List<DamageProjectileBehaviour>();
            _damageProjectilePrefab = damageProjectilePrefab;
            _totalStamina = totalStamina;
            _currentStamina = totalStamina;
            _staminaRefreshTime = staminaRefreshTime;
            _staminaRefreshTimeDelta = staminaRefreshTime;
        }

        protected override void InternalInitializeBattleActions()
        {
            _currentStamina = _totalStamina;
            ownerUnitBattleView.SetStaminaSlider(_currentStamina, _totalStamina);
        }

        protected override void InternalUpdateBattleActions()
        {
            _staminaRefreshTimeDelta -= Time.deltaTime;

            if (_staminaRefreshTimeDelta > 0) return;
        
            _staminaRefreshTimeDelta = _staminaRefreshTime;
            if (_currentStamina <= _totalStamina)
            {
                _currentStamina++;
                ownerUnitBattleView.SetStaminaSlider(_currentStamina, _totalStamina);
            }
        }

        protected override void InternalOnPerformAction()
        {
            if (ownerBattleBehaviour.CurrentState is not AttackState) return;
            
            if (_currentStamina <= 0) return;

            _currentStamina--;
            ownerUnitBattleView.SetStaminaSlider(_currentStamina, _totalStamina);

            NetworkedBattleBehaviour closestStats = ownerBattleBehaviour.GetTarget.Key;
            DamageProjectileBehaviour instantiatedProjectile = _damageProjectilePrefab.FireProjectile(
                ownerBattleBehaviour.transform.position,
                closestStats.transform.position, closestStats.PhotonView.ViewID);
            
            _instantiatedProjectiles.Add(instantiatedProjectile);
            instantiatedProjectile.RegisterOnCompleteAction(() =>
            {
                SendAttack(closestStats);
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
