using System.Collections.Generic;
using Features.Battle.Scripts;
using Features.Loot.Scripts.Generator;
using Features.Loot.Scripts.ModView;
using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Behaviours.Stat;
using UnityEngine;

namespace Features.Loot.Scripts.GeneratedLoot
{
    public class MultipleStatMod : BaseMod
    {
        private readonly List<MultipleStatModTarget> _multipleStatModTargets;
        private readonly BattleData_SO _battleData;
        private readonly NetworkedUnitRuntimeSet_SO _ownTeam;

        public MultipleStatMod(List<MultipleStatModTarget> multipleStatModTargets, BattleData_SO battleData, NetworkedUnitRuntimeSet_SO ownTeam, 
            GameObject spritePrefab, string description, int level, ModBehaviour modBehaviourPrefab) 
            : base(spritePrefab, description, level, modBehaviourPrefab)
        {
            _multipleStatModTargets = multipleStatModTargets;
            _battleData = battleData;
            _ownTeam = ownTeam;
        }

        public override void ApplyToInstantiatedUnit(NetworkedStatsBehaviour instantiatedUnit)
        {
            foreach (MultipleStatModTarget multipleStatModTarget in _multipleStatModTargets)
            {
                foreach (NetworkedBattleBehaviour manipulatedUnit in _battleData.GetTeam(_ownTeam, multipleStatModTarget.teamType))
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
                foreach (NetworkedBattleBehaviour manipulatedUnit in _battleData.GetTeam(_ownTeam, multipleStatModTarget.teamType))
                {
                    Add(manipulatedUnit.NetworkedStatsBehaviour, multipleStatModTarget.statType, multipleStatModTarget.baseValue, multipleStatModTarget.scaleValue);
                }
            }
        }
    
        protected override void InternalRemoveMod(NetworkedStatsBehaviour moddedLocalStats)
        {
            foreach (MultipleStatModTarget multipleStatModTarget in _multipleStatModTargets)
            {
                foreach (NetworkedBattleBehaviour manipulatedUnit in _battleData.GetTeam(_ownTeam, multipleStatModTarget.teamType))
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
