using System.Collections.Generic;
using Features.Loot.Scripts.Generator;
using Features.Loot.Scripts.ModView;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Behaviours.Stat;
using UnityEngine;

namespace Features.Loot.Scripts.GeneratedLoot
{
    public class MultipleStatMod : BaseMod
    {
        private readonly List<MultipleStatModTarget> _multipleStatModTargets;
        
        public MultipleStatMod(List<MultipleStatModTarget> multipleStatModTargets, string modName, string description, ModBehaviour modBehaviourPrefab) 
            : base(modName, description, modBehaviourPrefab)
        {
            _multipleStatModTargets = multipleStatModTargets;
        }

        public override void ApplyToInstantiatedUnit(NetworkedStatsBehaviour instantiatedUnit)
        {
            foreach (MultipleStatModTarget multipleStatModTarget in _multipleStatModTargets)
            {
                foreach (NetworkedBattleBehaviour manipulatedUnit in multipleStatModTarget.networkedUnitRuntimeSet.GetItems())
                {
                    if (manipulatedUnit.NetworkedStatsBehaviour == instantiatedUnit)
                    {
                        Add(instantiatedUnit, multipleStatModTarget.statType,
                            multipleStatModTarget.baseValue, multipleStatModTarget.scaleValue);
                    }
                }
            }
        }

        protected override void InternalAddMod(NetworkedStatsBehaviour moddedLocalStats, int slot)
        {
            foreach (MultipleStatModTarget multipleStatModTarget in _multipleStatModTargets)
            {
                foreach (NetworkedBattleBehaviour manipulatedUnit in multipleStatModTarget.networkedUnitRuntimeSet.GetItems())
                {
                    Add(manipulatedUnit.NetworkedStatsBehaviour, multipleStatModTarget.statType, multipleStatModTarget.baseValue, multipleStatModTarget.scaleValue);
                }
            }
        }
    
        protected override void InternalRemoveMod(NetworkedStatsBehaviour moddedLocalStats)
        {
            foreach (MultipleStatModTarget multipleStatModTarget in _multipleStatModTargets)
            {
                foreach (NetworkedBattleBehaviour manipulatedUnit in multipleStatModTarget.networkedUnitRuntimeSet.GetItems())
                {
                    Remove(manipulatedUnit.NetworkedStatsBehaviour, multipleStatModTarget.statType, multipleStatModTarget.baseValue, multipleStatModTarget.scaleValue);
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
