using System.Collections.Generic;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Behaviours.Stat;
using Features.Unit.Scripts.View;
using Photon.Pun;
using UnityEngine;

namespace Features.Unit.Scripts.Class
{
    public class CasterBattleClass : BattleClass
    {
        private readonly bool _isAi;
        private readonly DamageProjectileBehaviour _damageProjectilePrefab;
        private readonly List<DamageProjectileBehaviour> _instantiatedProjectiles;
        private float _attackSpeedDeltaTime;

        public CasterBattleClass(bool isAi, NetworkedStatsBehaviour ownerNetworkingStatsBehaviour,
            BattleBehaviour ownerBattleBehaviour,
            UnitBattleView ownerUnitBattleView, DamageProjectileBehaviour damageProjectilePrefab) : base(
            ownerNetworkingStatsBehaviour, ownerBattleBehaviour, ownerUnitBattleView)
        {
            _instantiatedProjectiles = new List<DamageProjectileBehaviour>();
            _isAi = isAi;
            _damageProjectilePrefab = damageProjectilePrefab;
            _attackSpeedDeltaTime = ownerNetworkingStatsBehaviour.NetworkedStatServiceLocator.GetTotalValue_MinIs1(StatType.Speed);
        }

        protected override void InternalInitializeBattleActions()
        {
            _attackSpeedDeltaTime = ownerNetworkingStatsBehaviour.NetworkedStatServiceLocator.GetTotalValue_MinIs1(StatType.Speed);
            ownerUnitBattleView.SetStaminaSlider(_attackSpeedDeltaTime, ownerNetworkingStatsBehaviour.NetworkedStatServiceLocator.GetTotalValue_MinIs1(StatType.Speed));
        }

        protected override void InternalUpdateBattleActions()
        {
            if (_isAi && !PhotonNetwork.IsMasterClient) return;

            _attackSpeedDeltaTime -= Time.deltaTime;
            
            if (_attackSpeedDeltaTime <= 0)
            {
                _attackSpeedDeltaTime = ownerNetworkingStatsBehaviour.NetworkedStatServiceLocator.GetTotalValue_MinIs1(StatType.Speed);
                InternalOnPerformAction();
            }
            
            ownerUnitBattleView.SetStaminaSlider(_attackSpeedDeltaTime, ownerNetworkingStatsBehaviour.NetworkedStatServiceLocator.GetTotalValue_MinIs1(StatType.Speed));
        }

        protected override void InternalOnPerformAction()
        {
            if (_isAi && !PhotonNetwork.IsMasterClient) return;
            
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
