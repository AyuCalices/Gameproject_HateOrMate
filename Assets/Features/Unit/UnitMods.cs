using System.Collections.Generic;
using Features.Mod;

namespace Features.Unit
{
    public class UnitMods
    {
        public ModSlot[] modSlots;

        public UnitMods(int size, UnitBehaviour unit)
        {
            modSlots = new ModSlot[size];

            for (int i = 0; i < size; i++)
            {
                modSlots[i] = new ModSlot(unit);
            }
        }
    }

    public class ModSlot
    {
        public BaseMod baseMod;
        public bool isActive;

        private UnitBehaviour unit;

        public ModSlot(UnitBehaviour unit)
        {
            this.unit = unit;
        }

        public bool ContainsMod()
        {
            return baseMod != null;
        }

        public void AddMod(BaseMod baseMod)
        {
            this.baseMod = baseMod;
            baseMod.EnableMod(unit);
        }

        public void RemoveMod()
        {
            baseMod.DisableMod(unit);
            baseMod = null;
        }

        public void UpdateActiveStatus()
        {
            if (isActive)
            {
                baseMod.EnableMod(unit);
            }
            else
            {
                baseMod.DisableMod(unit);
            }
        }

        public void DisableSlot()
        {
            isActive = false;

            if (baseMod != null)
            {
                baseMod.DisableMod(unit);
            }
        }
        
        public void EnableSlot()
        {
            isActive = true;
            
            if (baseMod != null)
            {
                baseMod.EnableMod(unit);
            }
        }
    }
}
