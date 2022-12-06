using System.Collections.Generic;
using Features.Battle;
using Features.GlobalReferences;
using Features.Unit.Modding;
using Features.Unit.View;
using UnityEngine;

namespace Features.Unit.Battle.Actions
{
    public class TowerBattleActions : BattleActions
    {
        private readonly DamageProjectileBehaviour _damageProjectilePrefab;
        private readonly List<DamageProjectileBehaviour> _instantiatedProjectiles;
        private readonly int _totalStamina;
        private int _currentStamina;
        private readonly float _staminaRefreshTime;
        private float _staminaRefreshTimeDelta;

        public TowerBattleActions(NetworkedUnitBehaviour ownerNetworkingUnitBehaviour, BattleBehaviour ownerBattleBehaviour,
            UnitView ownerUnitView, NetworkedUnitRuntimeSet_SO opponentNetworkedUnitRuntimeSet, DamageProjectileBehaviour damageProjectilePrefab, int totalStamina,
            float staminaRefreshTime) : base(ownerNetworkingUnitBehaviour, ownerBattleBehaviour, ownerUnitView,
            opponentNetworkedUnitRuntimeSet)
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
            ownerUnitView.SetStaminaSlider(_currentStamina, _totalStamina);
        }

        protected override void InternalUpdateBattleActions()
        {
            _staminaRefreshTimeDelta -= Time.deltaTime;

            if (_staminaRefreshTimeDelta > 0) return;
        
            _staminaRefreshTimeDelta = _staminaRefreshTime;
            if (_currentStamina <= _totalStamina)
            {
                _currentStamina++;
                ownerUnitView.SetStaminaSlider(_currentStamina, _totalStamina);
            }
        }

        protected override void InternalOnPerformAction()
        {
            if (ownerBattleBehaviour.CurrentState is not AttackState) return;
            
            if (_currentStamina <= 0) return;

            _currentStamina--;
            ownerUnitView.SetStaminaSlider(_currentStamina, _totalStamina);
        
            if (!ownerBattleBehaviour.TryGetTarget(out NetworkedUnitBehaviour closestUnit)) return;

            DamageProjectileBehaviour instantiatedProjectile = _damageProjectilePrefab.FireProjectile(
                ownerBattleBehaviour.transform.position,
                closestUnit.transform.position, closestUnit.PhotonView.ViewID);
            
            _instantiatedProjectiles.Add(instantiatedProjectile);
            instantiatedProjectile.RegisterOnCompleteAction(() =>
            {
                SendAttack(ownerNetworkingUnitBehaviour.ControlType, closestUnit);
                _instantiatedProjectiles.Remove(instantiatedProjectile);
            });
        }
        
        public override void OnStageEnd()
        {
            foreach (DamageProjectileBehaviour instantiatedProjectile in _instantiatedProjectiles)
            {
                instantiatedProjectile.CancelProjectile();
            }
                
            _instantiatedProjectiles.Clear();
        }
    }
}
