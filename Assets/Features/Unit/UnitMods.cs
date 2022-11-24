using System.Collections.Generic;
using Features.Mod;
using Features.ModHUD;
using UnityEngine;

namespace Features.Unit
{
    public class UnitMods
    {
        public readonly ModSlotContainer[] modSlotsContainers;
        public readonly List<ModSlotBehaviour> modSlotBehaviours;

        public UnitMods(int size, UnitBehaviour unit, List<ModSlotBehaviour> modSlotBehaviours)
        {
            this.modSlotBehaviours = modSlotBehaviours;
            
            modSlotsContainers = new ModSlotContainer[size];
            for (int i = 0; i < size; i++)
            {
                modSlotsContainers[i] = new ModSlotContainer(unit);
                modSlotBehaviours[i].Init(modSlotsContainers[i]);

                if (i > 2)
                {
                    modSlotsContainers[i].DisableSlot();
                }
            }
        }
        
        public void ToggleSlot(int index)
        {
            modSlotsContainers[index].ToggleSlot();
            modSlotBehaviours[index].UpdateSlot();
        }
    }

    public class ModSlotContainer
    {
        public BaseMod baseMod;
        public bool isActive;
        
        private UnitBehaviour unit;

        public ModSlotContainer(UnitBehaviour unit)
        {
            this.unit = unit;
            isActive = true;
        }

        public bool ContainsMod()
        {
            return baseMod != null;
        }

        public void AddOrExchangeMod(BaseMod newMod, ModSlotContainer origin)
        {
            BaseMod removedMod = baseMod;
            
            if (ContainsMod())
            {
                RemoveMod();
            }
            
            if (origin != null && origin.ContainsMod())
            {
                origin.RemoveMod();

                if (removedMod != null)
                {
                    origin.AddMod(removedMod);
                }
            }

            AddMod(newMod);
        }

        public void AddMod(BaseMod newMod)
        {
            baseMod = newMod;
            Debug.Log(isActive);

            if (isActive) newMod.EnableMod(unit);
        }

        public void RemoveMod()
        {
            if (isActive) baseMod.DisableMod(unit);
            
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
        
        public void ToggleSlot()
        {
            if (isActive)
            {
                DisableSlot();
            }
            else
            {
                EnableSlot();
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
