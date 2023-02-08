namespace Features.Unit.Scripts.Stats
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
