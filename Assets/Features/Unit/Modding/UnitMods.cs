using System.Collections.Generic;
using Features.Mod;
using Features.ModView;

namespace Features.Unit.Modding
{
    public class UnitMods
    {
        public readonly ModSlotContainer[] modSlotsContainers;
        public readonly List<ModSlotBehaviour> modSlotBehaviours;

        public UnitMods(int size, LocalUnitBehaviour localUnit, List<ModSlotBehaviour> modSlotBehaviours)
        {
            this.modSlotBehaviours = modSlotBehaviours;
            
            modSlotsContainers = new ModSlotContainer[size];
            for (int i = 0; i < size; i++)
            {
                modSlotsContainers[i] = new ModSlotContainer(localUnit);
                modSlotBehaviours[i].Init(modSlotsContainers[i]);

                if (i > 2)
                {
                    modSlotsContainers[i].DisableSlot();
                }
            }
        }

        public bool TryAddMod(ModDragBehaviour modDragBehaviour)
        {
            for (int index = 0; index < modSlotsContainers.Length; index++)
            {
                ModSlotContainer modSlotsContainer = modSlotsContainers[index];
                if (!modSlotsContainer.ContainsMod())
                {
                    modSlotsContainer.AddMod(modDragBehaviour.BaseMod);
                    modDragBehaviour.SetNewOrigin(modSlotsContainer, modSlotBehaviours[index]);

                    return true;
                }
            }

            return false;
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
        
        private LocalUnitBehaviour _localUnit;

        public ModSlotContainer(LocalUnitBehaviour localUnit)
        {
            this._localUnit = localUnit;
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

            if (isActive) newMod.EnableMod(_localUnit);
        }

        public void RemoveMod()
        {
            if (isActive) baseMod.DisableMod(_localUnit);
            
            baseMod = null;
        }

        public void UpdateActiveStatus()
        {
            if (isActive)
            {
                baseMod.EnableMod(_localUnit);
            }
            else
            {
                baseMod.DisableMod(_localUnit);
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
                baseMod.DisableMod(_localUnit);
            }
        }
        
        public void EnableSlot()
        {
            isActive = true;
            
            if (baseMod != null)
            {
                baseMod.EnableMod(_localUnit);
            }
        }
    }
}
