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

        public void EnableMod(UnitBehaviour moddedUnit)
        {
            if (_isEnabled) return;
            
            InternalAddMod(moddedUnit);
            _isEnabled = true;
        }
        
        public void DisableMod(UnitBehaviour moddedUnit)
        {
            if (!_isEnabled) return;
            
            InternalRemoveMod(moddedUnit);
            _isEnabled = false;
        }

        protected abstract void InternalAddMod(UnitBehaviour moddedUnit);
        protected abstract void InternalRemoveMod(UnitBehaviour moddedUnit);
    }
}
