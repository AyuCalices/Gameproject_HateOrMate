using Features.Unit;

namespace Features.Mod
{
    public abstract class BaseMod
    {
        public string ModName { get; }
        public string Description { get; }
    

        public BaseMod(string modName, string description)
        {
            ModName = modName;
            Description = description;
        }

        //make sure a mod can't be added twice
        public bool IsEquipped { get; private set; }

        public void EnableMod(UnitBehaviour moddedUnit)
        {
            if (IsEquipped) return;
            
            InternalAddMod(moddedUnit);
            IsEquipped = true;
        }
        
        public void DisableMod(UnitBehaviour moddedUnit)
        {
            if (!IsEquipped) return;
            
            InternalRemoveMod(moddedUnit);
            IsEquipped = false;
        }

        protected abstract void InternalAddMod(UnitBehaviour moddedUnit);
        protected abstract void InternalRemoveMod(UnitBehaviour moddedUnit);
    }
}
