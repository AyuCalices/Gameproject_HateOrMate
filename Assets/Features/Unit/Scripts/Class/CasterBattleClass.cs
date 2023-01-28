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
        private readonly SlashDamageAnimationBehaviour _damageAnimationPrefab;
        
        private float AttackSpeed => ownerNetworkingStatsBehaviour.NetworkedStatServiceLocator.GetTotalValue_CheckMin(StatType.Speed);
        private float _attackSpeedDeltaTime;

        public CasterBattleClass(bool isAi, NetworkedStatsBehaviour ownerNetworkingStatsBehaviour,
            BattleBehaviour ownerBattleBehaviour,
            UnitBattleView ownerUnitBattleView, SlashDamageAnimationBehaviour damageAnimationPrefab) : base(
            ownerNetworkingStatsBehaviour, ownerBattleBehaviour, ownerUnitBattleView)
        {
            _isAi = isAi;
            _damageAnimationPrefab = damageAnimationPrefab;
        }

        protected override void InternalInitializeBattleActions()
        {
            _attackSpeedDeltaTime = AttackSpeed;
            ownerUnitBattleView.SetStaminaSlider(_attackSpeedDeltaTime, AttackSpeed);
        }

        protected override void InternalUpdateBattleActions()
        {
            if (_isAi && !PhotonNetwork.IsMasterClient) return;

            _attackSpeedDeltaTime -= Time.deltaTime;
            
            if (_attackSpeedDeltaTime <= 0)
            {
                _attackSpeedDeltaTime = AttackSpeed;
                InternalOnPerformAction();
            }
            
            ownerUnitBattleView.SetStaminaSlider(_attackSpeedDeltaTime, AttackSpeed);
        }

        protected override void InternalOnPerformAction()
        {
            if (_isAi && !PhotonNetwork.IsMasterClient) return;
            
            NetworkedBattleBehaviour closestUnit = ownerBattleBehaviour.GetTarget.Key;
            _damageAnimationPrefab.InstantiateDamageAnimation(closestUnit.transform.position);
            SendAttack(closestUnit, ownerNetworkingStatsBehaviour.NetworkedStatServiceLocator.GetTotalValue_CheckMin(StatType.Damage));
        }

        public override void OnStageEnd()
        {
            ownerUnitBattleView.ResetStaminaSlider();
        }
    }
}
