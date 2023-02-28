using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Behaviours.Battle.BattleBehaviour;
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
        
        private float AttackSpeed => ownerUnitServiceProvider.GetService<NetworkedStatsBehaviour>().GetFinalStat(StatType.Speed);
        private float _attackSpeedDeltaTime;

        public CasterBattleClass(bool isAi, UnitServiceProvider ownerUnitServiceProvider, BaseDamageAnimationBehaviour baseDamageAnimationPrefab) : base(
            ownerUnitServiceProvider)
        {
            _isAi = isAi;
            _baseDamageAnimationPrefab = baseDamageAnimationPrefab;
        }

        protected override void InternalInitializeBattleActions()
        {
            _attackSpeedDeltaTime = AttackSpeed;
            ownerUnitServiceProvider.GetService<UnitBattleView>().SetStaminaSlider(_attackSpeedDeltaTime, AttackSpeed);
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
            
            ownerUnitServiceProvider.GetService<UnitBattleView>().SetStaminaSlider(_attackSpeedDeltaTime, AttackSpeed);
        }

        protected override void InternalOnPerformAction()
        {
            if (_isAi && !PhotonNetwork.IsMasterClient) return;
            
            if (ownerUnitServiceProvider.GetService<NetworkedBattleBehaviour>().BattleBehaviour is not ActiveBattleBehaviour activeBattleBehaviour) 
            {
                Debug.LogWarning($"OnPerformAction failed, because this Unit {ownerUnitServiceProvider.name} doesnt have an ActiveBattleBehaviour!");
                return;
            }
            
            UnitServiceProvider targetUnit = activeBattleBehaviour.GetTarget.Key;
            _baseDamageAnimationPrefab.InstantiateDamageAnimation(
                ownerUnitServiceProvider, targetUnit, () =>
                {
                    SendAttack(targetUnit, ownerUnitServiceProvider.GetService<NetworkedStatsBehaviour>().GetFinalStat(StatType.Damage));
                });
        }

        public override void OnStageEnd()
        {
            if (_isAi && !PhotonNetwork.IsMasterClient) return;
            
            BaseDamageAnimationBehaviour.DestroyAllByPrefabReference(_baseDamageAnimationPrefab, ownerUnitServiceProvider.GetService<PhotonView>());
        }
    }
}
