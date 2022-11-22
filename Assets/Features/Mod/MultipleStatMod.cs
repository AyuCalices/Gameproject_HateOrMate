using System;
using System.Collections.Generic;
using Features.Unit;
using Features.Unit.Stat;
using UnityEngine;

namespace Features.Mod
{
    public class MultipleStatMod : BaseMod
    {
        private readonly List<MultipleStatModTarget> _multipleStatModTargets;
        
        public MultipleStatMod(List<MultipleStatModTarget> multipleStatModTargets, string modName, string description) : base(modName, description)
        {
            _multipleStatModTargets = multipleStatModTargets;
        }
    
        protected override void InternalRemoveMod(UnitBehaviour moddedUnit)
        {
            foreach (MultipleStatModTarget multipleStatModTarget in _multipleStatModTargets)
            {
                switch (multipleStatModTarget.unitOwnerType)
                {
                    case UnitOwnerType.LocalPlayer:
                        foreach (UnitBehaviour manipulatedUnit in moddedUnit.LocalPlayerUnits.GetItems())
                        {
                            Apply(manipulatedUnit, multipleStatModTarget.statType, multipleStatModTarget.baseValue, multipleStatModTarget.scaleValue);
                        }
                        break;
                    case UnitOwnerType.ExternPlayer:
                        foreach (UnitBehaviour manipulatedUnit in moddedUnit.ExternPlayerUnits.GetItems())
                        {
                            Apply(manipulatedUnit, multipleStatModTarget.statType, multipleStatModTarget.baseValue, multipleStatModTarget.scaleValue);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        protected override void InternalAddMod(UnitBehaviour moddedUnit)
        {
            foreach (MultipleStatModTarget multipleStatModTarget in _multipleStatModTargets)
            {
                switch (multipleStatModTarget.unitOwnerType)
                {
                    case UnitOwnerType.LocalPlayer:
                        foreach (UnitBehaviour manipulatedUnit in moddedUnit.LocalPlayerUnits.GetItems())
                        {
                            Remove(manipulatedUnit, multipleStatModTarget.statType, multipleStatModTarget.baseValue, multipleStatModTarget.scaleValue);
                        }
                        break;
                    case UnitOwnerType.ExternPlayer:
                        foreach (UnitBehaviour manipulatedUnit in moddedUnit.ExternPlayerUnits.GetItems())
                        {
                            Remove(manipulatedUnit, multipleStatModTarget.statType, multipleStatModTarget.baseValue, multipleStatModTarget.scaleValue);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void Apply(UnitBehaviour unit, StatType statType, float baseValue, float scaleValue)
        {
            bool result = unit.NetworkedStatServiceLocator.TryAddLocalValue(statType, StatValueType.Stat, baseValue);
            if (!result)
            {
                Debug.LogWarning("Adding baseValue from Mod Failed!");
            }
        
            result = unit.NetworkedStatServiceLocator.TryAddLocalValue(statType, StatValueType.ScalingStat, scaleValue);
            if (!result)
            {
                Debug.LogWarning("Adding baseValue from Mod Failed!");
            }
        }

        private void Remove(UnitBehaviour unit, StatType statType, float baseValue, float scaleValue)
        {
            bool result = unit.NetworkedStatServiceLocator.TryRemoveLocalValue(statType, StatValueType.Stat, baseValue);
            if (!result)
            {
                Debug.LogWarning("Removing baseValue from Mod Failed!");
            }
        
            result = unit.NetworkedStatServiceLocator.TryRemoveLocalValue(statType, StatValueType.ScalingStat, scaleValue);
            if (!result)
            {
                Debug.LogWarning("Removing baseValue from Mod Failed!");
            }
        }
    }
}
