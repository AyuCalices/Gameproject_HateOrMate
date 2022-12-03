using System;
using System.Collections;
using Features.Battle;
using Features.GlobalReferences;
using Features.Unit.Modding;
using Features.Unit.View;
using Photon.Pun;
using UnityEngine;

namespace Features.Unit.Battle.Actions
{
    public class TowerBattleActions : BattleActions
    {
        private readonly DamageProjectileBehaviour _damageProjectileBehaviour;
        private readonly int _totalStamina;
        private int _currentStamina;
        private readonly float _staminaRefreshTime;
        private float _staminaRefreshTimeDelta;

        public TowerBattleActions(NetworkedUnitBehaviour ownerNetworkingUnitBehaviour, BattleBehaviour ownerBattleBehaviour,
            UnitView ownerUnitView, NetworkedUnitRuntimeSet_SO opponentNetworkedUnitRuntimeSet, DamageProjectileBehaviour damageProjectileBehaviour, int totalStamina,
            float staminaRefreshTime) : base(ownerNetworkingUnitBehaviour, ownerBattleBehaviour, ownerUnitView,
            opponentNetworkedUnitRuntimeSet)
        {
            _damageProjectileBehaviour = damageProjectileBehaviour;
            _totalStamina = totalStamina;
            _currentStamina = totalStamina;
            _staminaRefreshTime = staminaRefreshTime;
            _staminaRefreshTimeDelta = staminaRefreshTime;
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
            if (_currentStamina <= 0) return;

            _currentStamina--;
            ownerUnitView.SetStaminaSlider(_currentStamina, _totalStamina);
        
            if (!ownerBattleBehaviour.GetTarget(out NetworkedUnitBehaviour closestUnit)) return;

            ownerBattleBehaviour.StartCoroutine(
                FireProjectile(ownerBattleBehaviour.transform.position, closestUnit.transform.position,
                    () => SendAttack(ownerNetworkingUnitBehaviour.ControlType, closestUnit))
            );
        }
    
        private IEnumerator FireProjectile(Vector3 origin, Vector3 target, Action onComplete)
        {
            _damageProjectileBehaviour.PhotonInstantiate(origin, target);
            yield return new WaitForSeconds(ownerBattleBehaviour.damageProjectilePrefab.GetTime(origin, target));
            onComplete.Invoke();
        }
    }
}
