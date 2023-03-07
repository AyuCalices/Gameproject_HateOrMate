using System;
using System.Collections.Generic;
using Features.Battle.Scripts;
using Features.Loot.Scripts.Generator;
using Features.Loot.Scripts.ModView;
using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Stats;
using UnityEngine;

namespace Features.Loot.Scripts.GeneratedLoot
{
    public class MultipleStatMod : BaseMod
    {
        //events for registering stats, that arent bound to instantiated units (e.g. UI need them even though units aren't instantiated)
        public static Action<TeamTagType[], StatType, float> onRegisterGlobally;
        public static Action<TeamTagType[], StatType, float> onUnregisterGlobally;
        
        private readonly List<MultipleStatModTarget> _multipleStatModTargets;
        private readonly BattleData_SO _battleData;

        public MultipleStatMod(List<MultipleStatModTarget> multipleStatModTargets, BattleData_SO battleData, 
            GameObject spritePrefab, string description, int level, ModBehaviour modBehaviourPrefab) 
            : base(spritePrefab, description, level, modBehaviourPrefab)
        {
            _multipleStatModTargets = multipleStatModTargets;
            _battleData = battleData;
        }

        public override void ApplyToInstantiatedUnit(UnitServiceProvider instantiatedUnitServiceProvider)
        {
            foreach (MultipleStatModTarget multipleStatModTarget in _multipleStatModTargets)
            {
                //TODO: this is weird - maybe shorten
                foreach (UnitServiceProvider manipulatedUnit in _battleData.UnitsServiceProviderRuntimeSet.GetUnitsByTag(multipleStatModTarget.teamTagType))
                {
                    if (manipulatedUnit == instantiatedUnitServiceProvider)
                    {
                        Add(manipulatedUnit, multipleStatModTarget.statType,
                            multipleStatModTarget.baseValue, multipleStatModTarget.scaleValue, multipleStatModTarget.stageScaleValue);
                    }
                }
            }
        }

        protected override void InternalAddMod(UnitServiceProvider modifiedUnitServiceProvider, int slot)
        {
            foreach (MultipleStatModTarget multipleStatModTarget in _multipleStatModTargets)
            {
                foreach (UnitServiceProvider manipulatedUnit in _battleData.UnitsServiceProviderRuntimeSet.GetUnitsByTag(multipleStatModTarget.teamTagType))
                {
                    Add(manipulatedUnit, multipleStatModTarget.statType, multipleStatModTarget.baseValue, multipleStatModTarget.scaleValue, multipleStatModTarget.stageScaleValue);
                }
                
                onRegisterGlobally?.Invoke(
                    multipleStatModTarget.teamTagType, 
                    multipleStatModTarget.statType,
                    multipleStatModTarget.scaleValue
                    );
            }
        }
    
        protected override void InternalRemoveMod(UnitServiceProvider modifiedUnitServiceProvider)
        {
            foreach (MultipleStatModTarget multipleStatModTarget in _multipleStatModTargets)
            {
                onUnregisterGlobally?.Invoke(
                    multipleStatModTarget.teamTagType, 
                    multipleStatModTarget.statType,
                    -multipleStatModTarget.scaleValue
                );
                
                foreach (UnitServiceProvider manipulatedUnit in _battleData.UnitsServiceProviderRuntimeSet.GetUnitsByTag(multipleStatModTarget.teamTagType))
                {
                    Remove(manipulatedUnit, multipleStatModTarget.statType, multipleStatModTarget.baseValue, multipleStatModTarget.scaleValue, multipleStatModTarget.stageScaleValue);
                }
            }
        }

        private void Add(UnitServiceProvider modifiedUnitServiceProvider, StatType statType, float baseValue, float scaleValue, float stageScaleValue)
        {
            bool result = modifiedUnitServiceProvider.GetService<UnitStatsBehaviour>().NetworkedStatServiceLocator.TrySetStatValue<LocalModificationStat>(statType, StatValueType.Stat, ScaleByStage(baseValue, stageScaleValue));
            if (!result)
            {
                Debug.LogWarning("Adding baseValue from Mod Failed!");
            }
        
            result = modifiedUnitServiceProvider.GetService<UnitStatsBehaviour>().NetworkedStatServiceLocator.TrySetStatValue<LocalModificationStat>(statType, StatValueType.ScalingStat, scaleValue);
            if (!result)
            {
                Debug.LogWarning("Adding baseValue from Mod Failed!");
            }
        }

        private void Remove(UnitServiceProvider modifiedUnitServiceProvider, StatType statType, float baseValue, float scaleValue, float stageScaleValue)
        {
            bool result = modifiedUnitServiceProvider.GetService<UnitStatsBehaviour>().NetworkedStatServiceLocator.TryRemoveStatValue<LocalModificationStat>(statType, StatValueType.Stat, ScaleByStage(baseValue, stageScaleValue));
            if (!result)
            {
                Debug.LogWarning("Removing baseValue from Mod Failed!");
            }
        
            result = modifiedUnitServiceProvider.GetService<UnitStatsBehaviour>().NetworkedStatServiceLocator.TryRemoveStatValue<LocalModificationStat>(statType, StatValueType.ScalingStat, scaleValue);
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
