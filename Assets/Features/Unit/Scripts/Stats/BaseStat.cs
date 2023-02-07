using Features.Unit.Scripts.Behaviours.Stat;
using UnityEngine;

namespace Features.Unit.Scripts.Stats
{
    public class BaseStat : IUnitStat
    {
        private readonly float _baseValue;
        private readonly float _scaleValue;
        private readonly float _minValue;
        public StatType StatType { get; }

        public BaseStat(StatType statType, float baseValue, float minValue, float scaleValue = 1f)
        {
            StatType = statType;
            _baseValue = baseValue;
            _minValue = minValue;
            _scaleValue = scaleValue;
        }

        public float GetTotalValue()
        {
            return Mathf.Max(_baseValue * _scaleValue, _minValue);
        }

        public float GetMinValue()
        {
            return _minValue;
        }

        public float GetMultiplierStat()
        {
            return _scaleValue;
        }
    
        public float GetBaseStat()
        {
            return _baseValue;
        }
    }
}
