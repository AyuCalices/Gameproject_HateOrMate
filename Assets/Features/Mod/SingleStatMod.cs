using Features.Unit;
using Features.Unit.Stat;
using UnityEngine;

namespace Features.Mod
{
    public class SingleStatMod : BaseMod
    {
        private readonly StatType _statType;
        private readonly float _baseValue;
        private readonly float _scaleValue;
    
        public SingleStatMod(StatType statType, float baseValue, float scaleValue, string modName, string description) : base(modName, description)
        {
            _statType = statType;
            _baseValue = baseValue;
            _scaleValue = scaleValue;
        }
        
        protected override void InternalAddMod(UnitBehaviour moddedUnit)
        {
            bool result = moddedUnit.NetworkedStatServiceLocator.TryAddLocalValue(_statType, StatValueType.Stat, _baseValue);
            if (!result)
            {
                Debug.LogWarning("Removing baseValue from Mod Failed!");
            }
            
            result = moddedUnit.NetworkedStatServiceLocator.TryAddLocalValue(_statType, StatValueType.ScalingStat, _scaleValue);
            if (!result)
            {
                Debug.LogWarning("Removing baseValue from Mod Failed!");
            }
        }
    
        protected override void InternalRemoveMod(UnitBehaviour moddedUnit)
        {
            bool result = moddedUnit.NetworkedStatServiceLocator.TryRemoveLocalValue(_statType, StatValueType.Stat, _baseValue);
            if (!result)
            {
                Debug.LogWarning("Adding baseValue from Mod Failed!");
            }
            
            result = moddedUnit.NetworkedStatServiceLocator.TryRemoveLocalValue(_statType, StatValueType.ScalingStat, _scaleValue);
            if (!result)
            {
                Debug.LogWarning("Adding baseValue from Mod Failed!");
            }
        }
    }
}
