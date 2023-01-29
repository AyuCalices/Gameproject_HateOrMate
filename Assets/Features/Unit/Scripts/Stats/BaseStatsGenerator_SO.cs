using Features.Battle.Scripts.StageProgression;
using Features.Unit.Scripts.Behaviours.Stat;
using UnityEngine;

public abstract class BaseStatsGenerator_SO : ScriptableObject
{
    public abstract void ApplyBaseStats(NetworkedStatsBehaviour networkedStatsBehaviour, BaseStatsData_SO baseStatsData);
}
