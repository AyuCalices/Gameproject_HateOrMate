using System;
using Features.Loot.Scripts.ModView;
using Features.Unit.Scripts.Behaviours.Stat;
using UnityEngine;

namespace Features.Loot.Scripts.GeneratedLoot
{
    public abstract class BaseMod
    {
        public static Action<ModBehaviour, BaseMod> onModInstantiated;
        
        public string ModName { get; }
        public string Description { get; }
        
        //make sure a mod can't be added twice
        private bool _isEnabled;
        private readonly ModBehaviour _modBehaviourPrefab;

        protected BaseMod(string modName, string description, ModBehaviour modBehaviourPrefab)
        {
            _modBehaviourPrefab = modBehaviourPrefab;
            ModName = modName;
            Description = description;
            _isEnabled = false;
        }

        public void RaiseOnModInstantiated()
        {
            onModInstantiated?.Invoke(_modBehaviourPrefab, this);
        }

        public void EnableMod(NetworkedStatsBehaviour moddedLocalStats, int slot)
        {
            if (_isEnabled) { return; }
            
            InternalAddMod(moddedLocalStats, slot);
            _isEnabled = true;
        }
        
        public void DisableMod(NetworkedStatsBehaviour moddedLocalStats, bool isSwap)
        {
            if (isSwap) { return; }
            if (!_isEnabled) return;

            InternalRemoveMod(moddedLocalStats);
            _isEnabled = false;
        }

        public virtual bool IsValidAddMod(NetworkedStatsBehaviour instantiatedUnit, int slot) { return true; }
        public virtual void ApplyToInstantiatedUnit(NetworkedStatsBehaviour instantiatedUnit) {}
        protected abstract void InternalAddMod(NetworkedStatsBehaviour moddedLocalStats, int slot);
        protected abstract void InternalRemoveMod(NetworkedStatsBehaviour moddedLocalStats);
    }
}
