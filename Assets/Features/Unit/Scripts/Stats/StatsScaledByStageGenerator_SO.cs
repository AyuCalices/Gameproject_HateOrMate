using DataStructures.ReactiveVariable;
using Features.Unit.Scripts.Behaviours.Stat;
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
            
            networkedStatsBehaviour.NetworkedStatServiceLocator.SetBaseValue(StatType.Damage, finalAttack, baseStatsData.attackMinValue);
            networkedStatsBehaviour.NetworkedStatServiceLocator.SetBaseValue(StatType.Health, finalHealth, baseStatsData.healthMinValue);
            networkedStatsBehaviour.NetworkedStatServiceLocator.SetBaseValue(StatType.Speed, baseStatsData.speedValue, baseStatsData.speedMinValue);
            networkedStatsBehaviour.NetworkedStatServiceLocator.SetBaseValue(StatType.Range, baseStatsData.rangeValue, baseStatsData.rangeMinValue);
            networkedStatsBehaviour.NetworkedStatServiceLocator.SetBaseValue(StatType.MovementSpeed, baseStatsData.movementSpeedValue, baseStatsData.movementSpeedMinValue);
            networkedStatsBehaviour.NetworkedStatServiceLocator.SetBaseValue(StatType.Stamina, baseStatsData.staminaValue, baseStatsData.staminaMinValue);
            networkedStatsBehaviour.NetworkedStatServiceLocator.SetBaseValue(StatType.StaminaRefreshTime, baseStatsData.staminaRefreshTime, baseStatsData.staminaRefreshTimeMinValue);
        }
    }
}
