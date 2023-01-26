using Features.Battle.Scripts.StageProgression;
using Features.Unit.Scripts.Behaviours.Stat;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseStatsGenerator", menuName = "StageProgression/BaseStatsGenerator")]
public class StatsGenerator_SO : BaseStatsGenerator_SO
{
    public override void ApplyBaseStats(NetworkedStatsBehaviour networkedStatsBehaviour, BaseStatsData_SO baseStatsData)
    {
        networkedStatsBehaviour.NetworkedStatServiceLocator.SetBaseValue(StatType.Damage, baseStatsData.attackBaseValue, baseStatsData.attackMinValue);
        networkedStatsBehaviour.NetworkedStatServiceLocator.SetBaseValue(StatType.Health, baseStatsData.healthBaseValue, baseStatsData.healthMinValue);
        networkedStatsBehaviour.NetworkedStatServiceLocator.SetBaseValue(StatType.Speed, baseStatsData.speedValue, baseStatsData.speedMinValue);
        networkedStatsBehaviour.NetworkedStatServiceLocator.SetBaseValue(StatType.Range, baseStatsData.rangeValue, baseStatsData.rangeMinValue);
        networkedStatsBehaviour.NetworkedStatServiceLocator.SetBaseValue(StatType.MovementSpeed, baseStatsData.movementSpeedValue, baseStatsData.movementSpeedMinValue);
        networkedStatsBehaviour.NetworkedStatServiceLocator.SetBaseValue(StatType.Stamina, baseStatsData.staminaValue, baseStatsData.staminaMinValue);
        networkedStatsBehaviour.NetworkedStatServiceLocator.SetBaseValue(StatType.StaminaRefreshTime, baseStatsData.staminaRefreshTime, baseStatsData.staminaRefreshTimeMinValue);
    }
}
