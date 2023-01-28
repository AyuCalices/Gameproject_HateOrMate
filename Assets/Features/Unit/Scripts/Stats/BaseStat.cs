using Features.Unit.Scripts.Behaviours.Stat;

namespace Features.Unit.Scripts.Stats
{
    public class BaseStat : IUnitStat
    {
        private readonly float _baseValue;
        private readonly float _minValue;
        public StatType StatType { get; }

        public BaseStat(StatType statType, float baseValue, float minValue)
        {
            StatType = statType;
            _baseValue = baseValue;
            _minValue = minValue;
        }

        public float GetMinValue()
        {
            return _minValue;
        }
    
        public float GetTotalValue()
        {
            return _baseValue;
        }
    }
}
