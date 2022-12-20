using Features.Experimental;
using Features.Unit;
using Features.Unit.Modding;

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

        public void EnableMod(NetworkedUnitBehaviour moddedLocalUnit)
        {
            if (_isEnabled) return;
            
            InternalAddMod(moddedLocalUnit);
            _isEnabled = true;
        }
        
        public void DisableMod(NetworkedUnitBehaviour moddedLocalUnit)
        {
            if (!_isEnabled) return;
            
            InternalRemoveMod(moddedLocalUnit);
            _isEnabled = false;
        }

        

        protected abstract void InternalAddMod(NetworkedUnitBehaviour moddedLocalUnit);
        protected abstract void InternalRemoveMod(NetworkedUnitBehaviour moddedLocalUnit);
    }
}
