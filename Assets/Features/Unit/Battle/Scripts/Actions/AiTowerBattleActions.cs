using System.Collections.Generic;
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

        public AiTowerBattleActions(NetworkedStatsBehaviour ownerNetworkingStatsBehaviour,
            BattleBehaviour ownerBattleBehaviour,
            UnitView ownerUnitView, DamageProjectileBehaviour damageProjectilePrefab) : base(
            ownerNetworkingStatsBehaviour, ownerBattleBehaviour, ownerUnitView)
        {
            _instantiatedProjectiles = new List<DamageProjectileBehaviour>();
            _damageProjectilePrefab = damageProjectilePrefab;
            _attackSpeedDeltaTime = ownerNetworkingStatsBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Speed);
        }

        protected override void InternalInitializeBattleActions()
        {
            _attackSpeedDeltaTime = ownerNetworkingStatsBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Speed);
            ownerUnitView.SetStaminaSlider(_attackSpeedDeltaTime, ownerNetworkingStatsBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Speed));
        }

        protected override void InternalUpdateBattleActions()
        {
            if (!PhotonNetwork.IsMasterClient) return;

            _attackSpeedDeltaTime -= Time.deltaTime;
            
            if (_attackSpeedDeltaTime <= 0)
            {
                _attackSpeedDeltaTime = ownerNetworkingStatsBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Speed);
                InternalOnPerformAction();
            }
            
            ownerUnitView.SetStaminaSlider(_attackSpeedDeltaTime, ownerNetworkingStatsBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Speed));
        }

        protected override void InternalOnPerformAction()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            
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
            foreach (DamageProjectileBehaviour instantiatedProjectile in _instantiatedProjectiles)
            {
                instantiatedProjectile.CancelProjectile();
            }
                
            _instantiatedProjectiles.Clear();
        }
    }
}
