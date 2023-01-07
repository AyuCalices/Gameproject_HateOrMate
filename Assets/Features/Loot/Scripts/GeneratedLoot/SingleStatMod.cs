using Features.Loot.Scripts.ModView;
using Features.Unit.Scripts.Behaviours.Stat;
using UnityEngine;

namespace Features.Loot.Scripts.GeneratedLoot
{
    public class SingleStatMod : BaseMod
    {
        private readonly StatType _statType;
        private readonly float _baseValue;
        private readonly float _scaleValue;
        private readonly float _stageScaleValue;

        public SingleStatMod(StatType statType, float baseValue, float scaleValue, float stageScaleValue, GameObject spritePrefab, string description, int level, ModBehaviour modBehaviourPrefab) 
            : base(spritePrefab, description, level, modBehaviourPrefab)
        {
            _statType = statType;
            _baseValue = baseValue;
            _scaleValue = scaleValue;
            _stageScaleValue = stageScaleValue;
        }

        protected override void InternalAddMod(NetworkedStatsBehaviour moddedLocalStats, int slot)
        {
            bool result = moddedLocalStats.NetworkedStatServiceLocator.TryAddLocalValue(_statType, StatValueType.Stat, ScaleByStage(_baseValue));
            if (!result)
            {
                Debug.LogWarning("Adding value from Mod Failed!");
            }
            
            result = moddedLocalStats.NetworkedStatServiceLocator.TryAddLocalValue(_statType, StatValueType.ScalingStat, ScaleByStage(_scaleValue));
            if (!result)
            {
                Debug.LogWarning("Adding value from Mod Failed!");
            }
        }
    
        protected override void InternalRemoveMod(NetworkedStatsBehaviour moddedLocalStats)
        {
            bool result = moddedLocalStats.NetworkedStatServiceLocator.TryRemoveLocalValue(_statType, StatValueType.Stat, ScaleByStage(_baseValue));
            if (!result)
            {
                Debug.LogWarning("Removing value from Mod Failed!");
            }
            
            result = moddedLocalStats.NetworkedStatServiceLocator.TryRemoveLocalValue(_statType, StatValueType.ScalingStat, ScaleByStage(_scaleValue));
            if (!result)
            {
                Debug.LogWarning("Removing value from Mod Failed!");
            }
        }
        
        private float ScaleByStage(float value)
        {
            return value * Mathf.Pow(_stageScaleValue, Level);
        }
    }
}
