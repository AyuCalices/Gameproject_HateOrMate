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

        public SingleStatMod(StatType statType, float baseValue, float scaleValue, string modName, string description, ModDragBehaviour modDragBehaviourPrefab) 
            : base(modName, description, modDragBehaviourPrefab)
        {
            _statType = statType;
            _baseValue = baseValue;
            _scaleValue = scaleValue;
        }

        protected override void InternalAddMod(NetworkedStatsBehaviour moddedLocalStats)
        {
            Debug.Log("SingleStat");
            bool result = moddedLocalStats.NetworkedStatServiceLocator.TryAddLocalValue(_statType, StatValueType.Stat, _baseValue);
            if (!result)
            {
                Debug.LogWarning("Removing baseValue from Mod Failed!");
            }
            
            result = moddedLocalStats.NetworkedStatServiceLocator.TryAddLocalValue(_statType, StatValueType.ScalingStat, _scaleValue);
            if (!result)
            {
                Debug.LogWarning("Removing baseValue from Mod Failed!");
            }
        }
    
        protected override void InternalRemoveMod(NetworkedStatsBehaviour moddedLocalStats)
        {
            bool result = moddedLocalStats.NetworkedStatServiceLocator.TryRemoveLocalValue(_statType, StatValueType.Stat, _baseValue);
            if (!result)
            {
                Debug.LogWarning("Adding baseValue from Mod Failed!");
            }
            
            result = moddedLocalStats.NetworkedStatServiceLocator.TryRemoveLocalValue(_statType, StatValueType.ScalingStat, _scaleValue);
            if (!result)
            {
                Debug.LogWarning("Adding baseValue from Mod Failed!");
            }
        }
    }
}
