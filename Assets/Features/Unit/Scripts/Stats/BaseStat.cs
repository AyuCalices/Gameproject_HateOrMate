using System;
using UnityEngine;

namespace Features.Unit.Scripts.Stats
{
    public class BaseStat : IUnitStat, IChangeableStat
    {
        private float _baseValue;
        private float _multiplierValue;
        private float _minValue;
        public StatType StatType { get; }

        public BaseStat(StatType statType)
        {
            StatType = statType;
            _baseValue = 1f;
            _multiplierValue = 1f;
            _minValue = 1f;
        }

        public float GetMinValue()
        {
            return _minValue;
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
                case StatValueType.ScalingStat:
                    _multiplierValue = value;
                    break;
                case StatValueType.MinStat:
                    _minValue = value;
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
                    _baseValue = value;
                    return true;
                case StatValueType.ScalingStat:
                    _multiplierValue = value;
                    return true;
                case StatValueType.MinStat:
                    _minValue = value;
                    return true;
                default:
                    return false;
            }
        }
    }
}
