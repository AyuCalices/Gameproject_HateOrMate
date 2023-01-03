using System;
using Features.Loot.Scripts.ModView;
using Features.Unit.Scripts.Behaviours.Stat;

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

        public void EnableMod(NetworkedStatsBehaviour moddedLocalStats)
        {
            if (_isEnabled) return;
            
            InternalAddMod(moddedLocalStats);
            _isEnabled = true;
        }
        
        public void DisableMod(NetworkedStatsBehaviour moddedLocalStats, bool isSwap)
        {
            if (!_isEnabled) return;
            if (isSwap) return;
            
            InternalRemoveMod(moddedLocalStats);
            _isEnabled = false;
        }

        public virtual bool IsValidAddMod() { return true; }
        public virtual void ApplyToInstantiatedUnit(NetworkedStatsBehaviour instantiatedUnit) {}
        protected abstract void InternalAddMod(NetworkedStatsBehaviour moddedLocalStats);
        protected abstract void InternalRemoveMod(NetworkedStatsBehaviour moddedLocalStats);
    }
}