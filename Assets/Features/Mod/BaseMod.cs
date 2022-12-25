using Features.Unit.Battle.Scripts;
using Features.Unit.Modding;
using UnityEngine;

namespace Features.Mod
{
    public abstract class BaseMod
    {
        public string ModName { get; }
        public string Description { get; }
        
        //make sure a mod can't be added twice
        private bool _isEnabled;
        
        public BaseMod(string modName, string description)
        {
            ModName = modName;
            Description = description;
            _isEnabled = false;
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
        
        public virtual void ApplyToInstantiatedUnit(NetworkedStatsBehaviour instantiatedUnit) {}
        protected abstract void InternalAddMod(NetworkedStatsBehaviour moddedLocalStats);
        protected abstract void InternalRemoveMod(NetworkedStatsBehaviour moddedLocalStats);
    }
}
