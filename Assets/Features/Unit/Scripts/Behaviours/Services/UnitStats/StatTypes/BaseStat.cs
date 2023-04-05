using System;

namespace Features.Unit.Scripts.Behaviours.Services.UnitStats.StatTypes
{
    public class BaseStat : IUnitStat, IChangeableStat
    {
        private float _baseValue;
        private float _baseMinValue;
        private float _multiplierValue;
        private float _minMultiplierValue;
        
        public StatType StatType { get; }

        public BaseStat(StatType statType)
        {
            StatType = statType;
            _baseValue = 1f;
            _baseMinValue = 1f;
            _multiplierValue = 1f;
            _minMultiplierValue = 0.4f;
        }

        public float GetBaseMinValue()
        {
            return _baseMinValue;
        }
        
        public float GetMultiplierMinValue()
        {
            return _minMultiplierValue;
        }

        public float GetMultiplierStat()
        {
            return _multiplierValue;
        }
    
        public float GetBaseStat()
        {
            return _baseValue;
        }

        public void SetStatValue(StatValueType statValueType, float value)
        {
            switch (statValueType)
            {
                case StatValueType.Stat:
                    _baseValue = value;
                    break;
                case StatValueType.MinStat:
                    _baseMinValue = value;
                    break;
                case StatValueType.ScalingStat:
                    _multiplierValue = value;
                    break;
                case StatValueType.MinScalingStat:
                    _minMultiplierValue = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(statValueType), statValueType, null);
            }
        }

        public bool TryRemoveStatValue(StatValueType statValueType, float value = 0)
        {
            switch (statValueType)
            {
                case StatValueType.Stat:
                    _baseValue = 1;
                    return true;
                case StatValueType.ScalingStat:
                    _multiplierValue = 1;
                    return true;
                case StatValueType.MinStat:
                    _baseMinValue = 1;
                    return true;
                case StatValueType.MinScalingStat:
                    _minMultiplierValue = 0.4f;
                    return true;
                default:
                    return false;
            }
        }
    }
}
