using Features.Unit;

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

        public void EnableMod(LocalUnitBehaviour moddedLocalUnit)
        {
            if (_isEnabled) return;
            
            InternalAddMod(moddedLocalUnit);
            _isEnabled = true;
        }
        
        public void DisableMod(LocalUnitBehaviour moddedLocalUnit)
        {
            if (!_isEnabled) return;
            
            InternalRemoveMod(moddedLocalUnit);
            _isEnabled = false;
        }

        protected abstract void InternalAddMod(LocalUnitBehaviour moddedLocalUnit);
        protected abstract void InternalRemoveMod(LocalUnitBehaviour moddedLocalUnit);
    }
}
