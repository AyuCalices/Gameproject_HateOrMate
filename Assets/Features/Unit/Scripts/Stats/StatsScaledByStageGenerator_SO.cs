using DataStructures.ReactiveVariable;
using Features.Unit.Scripts.Behaviours.Stat;
using Features.Unit.Scripts.Stats;
using UnityEngine;

namespace Features.Battle.Scripts.StageProgression
{
    [CreateAssetMenu(fileName = "BaseStatsScaledByStageGenerator", menuName = "StageProgression/BaseStatsScaledByStageGenerator")]
    public class StatsScaledByStageGenerator_SO : BaseStatsGenerator_SO
    {
        [SerializeField] private IntReactiveVariable currentStage;
        
        [SerializeField] private float attackScalingByStages = 1.1f;
        [SerializeField] private float healthScalingByStages = 1.1f;
        
        public override void ApplyBaseStats(NetworkedStatsBehaviour networkedStatsBehaviour, BaseStatsData_SO baseStatsData)
        {
            float finalAttack = baseStatsData.attackBaseValue * Mathf.Pow(attackScalingByStages, currentStage.Get());
            float finalHealth = baseStatsData.healthBaseValue * Mathf.Pow(healthScalingByStages, currentStage.Get());
            
            networkedStatsBehaviour.NetworkedStatServiceLocator.Register(new BaseStat(StatType.Damage, finalAttack, baseStatsData.attackMinValue));
            networkedStatsBehaviour.NetworkedStatServiceLocator.Register(new BaseStat(StatType.Health, finalHealth, baseStatsData.healthMinValue));
            networkedStatsBehaviour.NetworkedStatServiceLocator.Register(new BaseStat(StatType.Speed, baseStatsData.speedValue, baseStatsData.speedMinValue));
            networkedStatsBehaviour.NetworkedStatServiceLocator.Register(new BaseStat(StatType.Range, baseStatsData.rangeValue, baseStatsData.rangeMinValue));
            networkedStatsBehaviour.NetworkedStatServiceLocator.Register(new BaseStat(StatType.MovementSpeed, baseStatsData.movementSpeedValue, baseStatsData.movementSpeedMinValue));
            networkedStatsBehaviour.NetworkedStatServiceLocator.Register(new BaseStat(StatType.Stamina, baseStatsData.staminaValue, baseStatsData.staminaMinValue));
        }
    }
}
