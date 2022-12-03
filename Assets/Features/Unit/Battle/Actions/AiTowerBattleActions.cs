using System;
using System.Collections;
using Features.Battle;
using Features.GlobalReferences;
using Features.Unit.Modding;
using Features.Unit.Modding.Stat;
using Features.Unit.View;
using Photon.Pun;
using UnityEngine;

namespace Features.Unit.Battle.Actions
{
    public class AiTowerBattleActions : BattleActions
    {
        private readonly DamageProjectileBehaviour _damageProjectileBehaviour;
        private float _attackSpeedDeltaTime;

        public AiTowerBattleActions(NetworkedUnitBehaviour ownerNetworkingUnitBehaviour,
            BattleBehaviour ownerBattleBehaviour,
            UnitView ownerUnitView, NetworkedUnitRuntimeSet_SO opponentNetworkedUnitRuntimeSet, DamageProjectileBehaviour damageProjectileBehaviour) : base(
            ownerNetworkingUnitBehaviour, ownerBattleBehaviour, ownerUnitView,
            opponentNetworkedUnitRuntimeSet)
        {
            _damageProjectileBehaviour = damageProjectileBehaviour;
            _attackSpeedDeltaTime = ownerNetworkingUnitBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Speed);
        }

        protected override void InternalUpdateBattleActions()
        {
            if (!PhotonNetwork.IsMasterClient) return;

            _attackSpeedDeltaTime -= Time.deltaTime;
            
            if (_attackSpeedDeltaTime <= 0)
            {
                _attackSpeedDeltaTime = ownerNetworkingUnitBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Speed);
                InternalOnPerformAction();
            }
            
            //ownerUnitView.SetStaminaSlider(_attackSpeedDeltaTime, ownerNetworkingUnitBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Speed));
        }

        protected override void InternalOnPerformAction()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            
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
