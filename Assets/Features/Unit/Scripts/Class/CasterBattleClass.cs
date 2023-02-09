using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.DamageAnimation;
using Features.Unit.Scripts.Stats;
using Features.Unit.Scripts.View;
using Photon.Pun;
using UnityEngine;

namespace Features.Unit.Scripts.Class
{
    public class CasterBattleClass : BattleClass
    {
        private readonly bool _isAi;
        private readonly BaseDamageAnimationBehaviour _baseDamageAnimationPrefab;
        
        private float AttackSpeed => ownerNetworkingStatsBehaviour.GetFinalStat(StatType.Speed);
        private float _attackSpeedDeltaTime;

        public CasterBattleClass(bool isAi, NetworkedStatsBehaviour ownerNetworkingStatsBehaviour,
            BattleBehaviour ownerBattleBehaviour, UnitBattleView ownerUnitBattleView, BaseDamageAnimationBehaviour baseDamageAnimationPrefab) : base(
            ownerNetworkingStatsBehaviour, ownerBattleBehaviour, ownerUnitBattleView)
        {
            _isAi = isAi;
            _baseDamageAnimationPrefab = baseDamageAnimationPrefab;
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
            
            NetworkedBattleBehaviour targetUnit = ownerBattleBehaviour.GetTarget.Key;
            _baseDamageAnimationPrefab.InstantiateDamageAnimation(
                ownerBattleBehaviour, targetUnit, () =>
                {
                    SendAttack(targetUnit, ownerNetworkingStatsBehaviour.GetFinalStat(StatType.Damage));
                });
        }

        public override void OnStageEnd()
        {
            if (_isAi && !PhotonNetwork.IsMasterClient) return;
            
            BaseDamageAnimationBehaviour.DestroyAllByPrefabReference(_baseDamageAnimationPrefab, ownerBattleBehaviour.PhotonView);
        }
    }
}
