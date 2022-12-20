using System;
using System.Collections.Generic;
using Features.Loot;
using Features.Loot.Scripts;
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

        protected override void InternalAddMod(NetworkedStatsBehaviour moddedLocalStats)
        {
            foreach (MultipleStatModTarget multipleStatModTarget in _multipleStatModTargets)
            {
                foreach (NetworkedStatsBehaviour manipulatedUnit in multipleStatModTarget.networkedUnitRuntimeSet.GetItems())
                {
                    Add(manipulatedUnit, multipleStatModTarget.statType, multipleStatModTarget.baseValue, multipleStatModTarget.scaleValue);
                }
            }
        }
    
        protected override void InternalRemoveMod(NetworkedStatsBehaviour moddedLocalStats)
        {
            foreach (MultipleStatModTarget multipleStatModTarget in _multipleStatModTargets)
            {
                foreach (NetworkedStatsBehaviour manipulatedUnit in multipleStatModTarget.networkedUnitRuntimeSet.GetItems())
                {
                    Remove(manipulatedUnit, multipleStatModTarget.statType, multipleStatModTarget.baseValue, multipleStatModTarget.scaleValue);
                }
            }
        }

        private void Add(NetworkedStatsBehaviour modifiedStats, StatType statType, float baseValue, float scaleValue)
        {
            bool result = modifiedStats.NetworkedStatServiceLocator.TryAddLocalValue(statType, StatValueType.Stat, baseValue);
            if (!result)
            {
                Debug.LogWarning("Adding baseValue from Mod Failed!");
            }
        
            result = modifiedStats.NetworkedStatServiceLocator.TryAddLocalValue(statType, StatValueType.ScalingStat, scaleValue);
            if (!result)
            {
                Debug.LogWarning("Adding baseValue from Mod Failed!");
            }
        }

        private void Remove(NetworkedStatsBehaviour modifiedStats, StatType statType, float baseValue, float scaleValue)
        {
            bool result = modifiedStats.NetworkedStatServiceLocator.TryRemoveLocalValue(statType, StatValueType.Stat, baseValue);
            if (!result)
            {
                Debug.LogWarning("Removing baseValue from Mod Failed!");
            }
        
            result = modifiedStats.NetworkedStatServiceLocator.TryRemoveLocalValue(statType, StatValueType.ScalingStat, scaleValue);
            if (!result)
            {
                Debug.LogWarning("Removing baseValue from Mod Failed!");
            }
        }
    }
}
