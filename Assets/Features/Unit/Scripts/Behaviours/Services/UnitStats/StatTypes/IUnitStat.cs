namespace Features.Unit.Scripts.Behaviours.Services.UnitStats.StatTypes
{
    public interface IUnitStat
    { 
        StatType StatType { get; }

        float GetBaseStat();
        
        float GetMultiplierStat();
    }

    public interface IChangeableStat
    {
        void SetStatValue(StatValueType statValueType, float value);

        bool TryRemoveStatValue(StatValueType statValueType, float value = 0);
    }
}
