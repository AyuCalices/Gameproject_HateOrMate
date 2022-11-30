using System;
using System.Collections.Generic;
using Features.Unit;
using Features.Unit.Modding;
using Features.Unit.Modding.Stat;
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
        
        protected override void InternalAddMod(LocalUnitBehaviour moddedLocalUnit)
        {
            foreach (MultipleStatModTarget multipleStatModTarget in _multipleStatModTargets)
            {
                switch (multipleStatModTarget.unitOwnerType)
                {
                    case UnitOwnerType.LocalPlayer:
                        foreach (LocalUnitBehaviour manipulatedUnit in moddedLocalUnit.LocalPlayerLocalUnits.GetItems())
                        {
                            Add(manipulatedUnit, multipleStatModTarget.statType, multipleStatModTarget.baseValue, multipleStatModTarget.scaleValue);
                        }
                        break;
                    case UnitOwnerType.ExternPlayer:
                        foreach (NetworkedUnitBehaviour manipulatedUnit in moddedLocalUnit.NetworkedPlayerUnits.GetItems())
                        {
                            Add(manipulatedUnit, multipleStatModTarget.statType, multipleStatModTarget.baseValue, multipleStatModTarget.scaleValue);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    
        protected override void InternalRemoveMod(LocalUnitBehaviour moddedLocalUnit)
        {
            foreach (MultipleStatModTarget multipleStatModTarget in _multipleStatModTargets)
            {
                switch (multipleStatModTarget.unitOwnerType)
                {
                    case UnitOwnerType.LocalPlayer:
                        foreach (LocalUnitBehaviour manipulatedUnit in moddedLocalUnit.LocalPlayerLocalUnits.GetItems())
                        {
                            Remove(manipulatedUnit, multipleStatModTarget.statType, multipleStatModTarget.baseValue, multipleStatModTarget.scaleValue);
                        }
                        break;
                    case UnitOwnerType.ExternPlayer:
                        foreach (NetworkedUnitBehaviour manipulatedUnit in moddedLocalUnit.NetworkedPlayerUnits.GetItems())
                        {
                            Remove(manipulatedUnit, multipleStatModTarget.statType, multipleStatModTarget.baseValue, multipleStatModTarget.scaleValue);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void Add(NetworkedUnitBehaviour localUnit, StatType statType, float baseValue, float scaleValue)
        {
            bool result = localUnit.NetworkedStatServiceLocator.TryAddLocalValue(statType, StatValueType.Stat, baseValue);
            if (!result)
            {
                Debug.LogWarning("Adding baseValue from Mod Failed!");
            }
        
            result = localUnit.NetworkedStatServiceLocator.TryAddLocalValue(statType, StatValueType.ScalingStat, scaleValue);
            if (!result)
            {
                Debug.LogWarning("Adding baseValue from Mod Failed!");
            }
        }

        private void Remove(NetworkedUnitBehaviour localUnit, StatType statType, float baseValue, float scaleValue)
        {
            bool result = localUnit.NetworkedStatServiceLocator.TryRemoveLocalValue(statType, StatValueType.Stat, baseValue);
            if (!result)
            {
                Debug.LogWarning("Removing baseValue from Mod Failed!");
            }
        
            result = localUnit.NetworkedStatServiceLocator.TryRemoveLocalValue(statType, StatValueType.ScalingStat, scaleValue);
            if (!result)
            {
                Debug.LogWarning("Removing baseValue from Mod Failed!");
            }
        }
    }
}
