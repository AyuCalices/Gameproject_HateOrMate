using System.Collections.Generic;
using Features.GlobalReferences.Scripts;
using Features.Unit.Modding;
using Features.Unit.Modding.Stat;
using Features.Unit.View;
using Photon.Pun;
using UnityEngine;

namespace Features.Unit.Battle.Scripts.Actions
{
    public class AiTowerBattleActions : BattleActions
    {
        private readonly DamageProjectileBehaviour _damageProjectilePrefab;
        private readonly List<DamageProjectileBehaviour> _instantiatedProjectiles;
        private float _attackSpeedDeltaTime;

        public AiTowerBattleActions(NetworkedUnitBehaviour ownerNetworkingUnitBehaviour,
            BattleBehaviour ownerBattleBehaviour,
            UnitView ownerUnitView, NetworkedUnitRuntimeSet_SO opponentNetworkedUnitRuntimeSet, DamageProjectileBehaviour damageProjectilePrefab) : base(
            ownerNetworkingUnitBehaviour, ownerBattleBehaviour, ownerUnitView,
            opponentNetworkedUnitRuntimeSet)
        {
            _instantiatedProjectiles = new List<DamageProjectileBehaviour>();
            _damageProjectilePrefab = damageProjectilePrefab;
            _attackSpeedDeltaTime = ownerNetworkingUnitBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Speed);
        }

        protected override void InternalInitializeBattleActions()
        {
            _attackSpeedDeltaTime = ownerNetworkingUnitBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Speed);
            ownerUnitView.SetStaminaSlider(_attackSpeedDeltaTime, ownerNetworkingUnitBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Speed));
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
            
            ownerUnitView.SetStaminaSlider(_attackSpeedDeltaTime, ownerNetworkingUnitBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Speed));
        }

        protected override void InternalOnPerformAction()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            
            NetworkedUnitBehaviour closestUnit = ownerBattleBehaviour.GetTarget.Key;
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
