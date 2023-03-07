using System;
using Features.Mods.Scripts.View;
using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Services.UnitStats;
using Features.Unit.Scripts.Behaviours.Services.UnitStats.StatTypes;
using UnityEngine;

namespace Features.Mods.Scripts.ModTypes
{
    public class SingleStatMod : BaseMod
    {
        public static Action<UnitStatsBehaviour, StatType, float, float> onRegister;
        public static Action<UnitStatsBehaviour, StatType, float, float> onUnregister;
        
        private readonly StatType _statType;
        private readonly float _baseValue;
        private readonly float _scaleValue;
        private readonly float _stageScaleValue;

        public SingleStatMod(StatType statType, float baseValue, float scaleValue, float stageScaleValue, GameObject spritePrefab, string description, int level, ModViewBehaviour modViewBehaviourPrefab) 
            : base(spritePrefab, description, level, modViewBehaviourPrefab)
        {
            _statType = statType;
            _baseValue = baseValue;
            _scaleValue = scaleValue;
            _stageScaleValue = stageScaleValue;
        }

        protected override void InternalAddMod(UnitServiceProvider modifiedUnitServiceProvider, int slot)
        {
            bool result = modifiedUnitServiceProvider.GetService<UnitStatsBehaviour>().StatServiceLocator.TrySetStatValue<LocalModificationStat>(_statType, StatValueType.Stat, ScaleByStage(_baseValue));
            if (!result)
            {
                Debug.LogWarning("Adding value from Mod Failed!");
            }
            
            result = modifiedUnitServiceProvider.GetService<UnitStatsBehaviour>().StatServiceLocator.TrySetStatValue<LocalModificationStat>(_statType, StatValueType.ScalingStat, _scaleValue);
            if (!result)
            {
                Debug.LogWarning("Adding value from Mod Failed!");
            }
            else
            {
                onRegister?.Invoke(modifiedUnitServiceProvider.GetService<UnitStatsBehaviour>(), _statType, ScaleByStage(_baseValue), _scaleValue);
            }
        }
    
        protected override void InternalRemoveMod(UnitServiceProvider modifiedUnitServiceProvider)
        {
            bool result = modifiedUnitServiceProvider.GetService<UnitStatsBehaviour>().StatServiceLocator.TryRemoveStatValue<LocalModificationStat>(_statType, StatValueType.Stat, ScaleByStage(_baseValue));
            if (!result)
            {
                Debug.LogWarning("Removing value from Mod Failed!");
            }
            
            result = modifiedUnitServiceProvider.GetService<UnitStatsBehaviour>().StatServiceLocator.TryRemoveStatValue<LocalModificationStat>(_statType, StatValueType.ScalingStat, _scaleValue);
            if (!result)
            {
                Debug.LogWarning("Removing value from Mod Failed!");
            }
            else
            {
                onUnregister?.Invoke(modifiedUnitServiceProvider.GetService<UnitStatsBehaviour>(), _statType, -ScaleByStage(_baseValue), -_scaleValue);
            }
        }
        
        private float ScaleByStage(float value)
        {
            return value * Mathf.Pow(_stageScaleValue, Level);
        }
    }
}
