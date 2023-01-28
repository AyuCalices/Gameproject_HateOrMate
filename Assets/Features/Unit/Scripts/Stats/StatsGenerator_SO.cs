using Features.Battle.Scripts.StageProgression;
using Features.Unit.Scripts.Behaviours.Stat;
using Features.Unit.Scripts.Stats;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseStatsGenerator", menuName = "StageProgression/BaseStatsGenerator")]
public class StatsGenerator_SO : BaseStatsGenerator_SO
{
    public override void ApplyBaseStats(NetworkedStatsBehaviour networkedStatsBehaviour, BaseStatsData_SO baseStatsData)
    {
        networkedStatsBehaviour.NetworkedStatServiceLocator.Register(new BaseStat(StatType.Damage, baseStatsData.attackBaseValue, baseStatsData.attackMinValue));
        networkedStatsBehaviour.NetworkedStatServiceLocator.Register(new BaseStat(StatType.Health, baseStatsData.healthBaseValue, baseStatsData.healthMinValue));
        networkedStatsBehaviour.NetworkedStatServiceLocator.Register(new BaseStat(StatType.Speed, baseStatsData.speedValue, baseStatsData.speedMinValue));
        networkedStatsBehaviour.NetworkedStatServiceLocator.Register(new BaseStat(StatType.Range, baseStatsData.rangeValue, baseStatsData.rangeMinValue));
        networkedStatsBehaviour.NetworkedStatServiceLocator.Register(new BaseStat(StatType.MovementSpeed, baseStatsData.movementSpeedValue, baseStatsData.movementSpeedMinValue));
        networkedStatsBehaviour.NetworkedStatServiceLocator.Register(new BaseStat(StatType.Stamina, baseStatsData.staminaValue, baseStatsData.staminaMinValue));
    }
}
