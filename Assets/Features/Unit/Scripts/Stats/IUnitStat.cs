namespace Features.Unit.Scripts.Behaviours.Stat
{
    public interface IUnitStat
    { 
        StatType StatType { get; }

        float GetBaseStat();
        
        float GetMultiplierStat();
    }
}
