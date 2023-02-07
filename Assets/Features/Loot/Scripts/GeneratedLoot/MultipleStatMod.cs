using System;
using System.Collections.Generic;
using Features.Battle.Scripts;
using Features.Loot.Scripts.Generator;
using Features.Loot.Scripts.ModView;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Behaviours.Stat;
using UnityEngine;

namespace Features.Loot.Scripts.GeneratedLoot
{
    public class MultipleStatMod : BaseMod
    {
        //events for registering stats, that arent bound to instantiated units (e.g. UI need them even though units aren't instantiated)
        public static Action<TeamTagType[], StatType, float, float> onRegisterGlobally;
        public static Action<TeamTagType[], StatType, float, float> onUnregisterGlobally;
        
        private readonly List<MultipleStatModTarget> _multipleStatModTargets;
        private readonly BattleData_SO _battleData;

        public MultipleStatMod(List<MultipleStatModTarget> multipleStatModTargets, BattleData_SO battleData, 
            GameObject spritePrefab, string description, int level, ModBehaviour modBehaviourPrefab) 
            : base(spritePrefab, description, level, modBehaviourPrefab)
        {
            _multipleStatModTargets = multipleStatModTargets;
            _battleData = battleData;
        }

        public override void ApplyToInstantiatedUnit(NetworkedStatsBehaviour instantiatedUnit)
        {
            foreach (MultipleStatModTarget multipleStatModTarget in _multipleStatModTargets)
            {
                foreach (NetworkedBattleBehaviour manipulatedUnit in _battleData.AllUnitsRuntimeSet.GetUnitsByTag(multipleStatModTarget.teamTagType))
                {
                    if (manipulatedUnit.NetworkedStatsBehaviour == instantiatedUnit)
                    {
                        Add(manipulatedUnit, multipleStatModTarget.statType,
                            multipleStatModTarget.baseValue, multipleStatModTarget.scaleValue, multipleStatModTarget.stageScaleValue);
                    }
                }
            }
        }

        protected override void InternalAddMod(NetworkedStatsBehaviour moddedLocalStats, int slot)
        {
            foreach (MultipleStatModTarget multipleStatModTarget in _multipleStatModTargets)
            {
                foreach (NetworkedBattleBehaviour manipulatedUnit in _battleData.AllUnitsRuntimeSet.GetUnitsByTag(multipleStatModTarget.teamTagType))
                {
                    Add(manipulatedUnit, multipleStatModTarget.statType, multipleStatModTarget.baseValue, multipleStatModTarget.scaleValue, multipleStatModTarget.stageScaleValue);
                }
                
                onRegisterGlobally?.Invoke(
                    multipleStatModTarget.teamTagType, 
                    multipleStatModTarget.statType, 
                    ScaleByStage(multipleStatModTarget.baseValue, multipleStatModTarget.stageScaleValue), 
                    multipleStatModTarget.scaleValue
                    );
            }
        }
    
        protected override void InternalRemoveMod(NetworkedStatsBehaviour moddedLocalStats)
        {
            foreach (MultipleStatModTarget multipleStatModTarget in _multipleStatModTargets)
            {
                onUnregisterGlobally?.Invoke(
                    multipleStatModTarget.teamTagType, 
                    multipleStatModTarget.statType, 
                    -ScaleByStage(multipleStatModTarget.baseValue, multipleStatModTarget.stageScaleValue), 
                    -multipleStatModTarget.scaleValue
                );
                
                foreach (NetworkedBattleBehaviour manipulatedUnit in _battleData.AllUnitsRuntimeSet.GetUnitsByTag(multipleStatModTarget.teamTagType))
                {
                    Remove(manipulatedUnit, multipleStatModTarget.statType, multipleStatModTarget.baseValue, multipleStatModTarget.scaleValue, multipleStatModTarget.stageScaleValue);
                }
            }
        }

        private void Add(NetworkedBattleBehaviour networkedBattleBehaviour, StatType statType, float baseValue, float scaleValue, float stageScaleValue)
        {
            bool result = networkedBattleBehaviour.NetworkedStatsBehaviour.NetworkedStatServiceLocator.TryAddLocalValue(statType, StatValueType.Stat, ScaleByStage(baseValue, stageScaleValue));
            if (!result)
            {
                Debug.LogWarning("Adding baseValue from Mod Failed!");
            }
        
            result = networkedBattleBehaviour.NetworkedStatsBehaviour.NetworkedStatServiceLocator.TryAddLocalValue(statType, StatValueType.ScalingStat, scaleValue);
            if (!result)
            {
                Debug.LogWarning("Adding baseValue from Mod Failed!");
            }
        }

        private void Remove(NetworkedBattleBehaviour networkedBattleBehaviour, StatType statType, float baseValue, float scaleValue, float stageScaleValue)
        {
            bool result = networkedBattleBehaviour.NetworkedStatsBehaviour.NetworkedStatServiceLocator.TryRemoveLocalValue(statType, StatValueType.Stat, ScaleByStage(baseValue, stageScaleValue));
            if (!result)
            {
                Debug.LogWarning("Removing baseValue from Mod Failed!");
            }
        
            result = networkedBattleBehaviour.NetworkedStatsBehaviour.NetworkedStatServiceLocator.TryRemoveLocalValue(statType, StatValueType.ScalingStat, scaleValue);
            if (!result)
            {
                Debug.LogWarning("Removing baseValue from Mod Failed!");
            }
        }
        
        private float ScaleByStage(float value, float stageScaleValue)
        {
            return value * Mathf.Pow(stageScaleValue, Level);
        }
    }
}
