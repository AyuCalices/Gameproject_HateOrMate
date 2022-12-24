using System.Collections.Generic;
using Features.Mod;
using Features.ModView;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Unit.Modding
{
    public class UnitMods
    {
        public readonly ModSlotContainer[] modSlotsContainers;
        public readonly List<ModSlotBehaviour> modSlotBehaviours;

        public UnitMods(int size, NetworkedStatsBehaviour localStats, List<ModSlotBehaviour> modSlotBehaviours)
        {
            this.modSlotBehaviours = modSlotBehaviours;
            
            modSlotsContainers = new ModSlotContainer[size];
            for (int i = 0; i < size; i++)
            {
                modSlotsContainers[i] = new ModSlotContainer(localStats);
                modSlotBehaviours[i].Init(modSlotsContainers[i]);

                if (i > 2)
                {
                    modSlotsContainers[i].DisableSlot();
                }
            }
        }
        
        public bool TryInstantiateMod(ModDragBehaviour modDragBehaviourPrefab, BaseMod baseMod)
        {
            for (int index = 0; index < modSlotsContainers.Length; index++)
            {
                ModSlotContainer modSlotsContainer = modSlotsContainers[index];
                if (!modSlotsContainer.ContainsMod())
                {
                    ModDragBehaviour modDragBehaviour = Object.Instantiate(modDragBehaviourPrefab, modSlotBehaviours[index].transform);
                    modDragBehaviour.Initialize(modSlotsContainer, modSlotBehaviours[index], baseMod);
                    modSlotsContainer.AddMod(baseMod);
                    return true;
                }
            }

            return false;
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
        
        private NetworkedStatsBehaviour _localStats;

        public ModSlotContainer(NetworkedStatsBehaviour localStats)
        {
            this._localStats = localStats;
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
                RemoveMod(true);
            }
            
            if (origin.ContainsMod())
            {
                origin.RemoveMod(true);

                if (removedMod != null)
                {
                    origin.SwapAddMod(removedMod, origin);
                }
            }

            SwapAddMod(newMod, origin);
        }

        private void SwapAddMod(BaseMod newMod, ModSlotContainer origin)
        {
            if (!isActive)
            {
                newMod.DisableMod(_localStats, false);
                origin.baseMod = null;
            }
            else if (isActive)
            {
                AddMod(newMod);
            }
        }

        public void AddMod(BaseMod newMod)
        {
            baseMod = newMod;

            if (isActive) newMod.EnableMod(_localStats);
        }
        
        private void RemoveMod(bool isSwap)
        {
            if (isActive) baseMod.DisableMod(_localStats, isSwap);
            
            baseMod = null;
        }

        public void UpdateActiveStatus()
        {
            if (isActive)
            {
                baseMod.EnableMod(_localStats);
            }
            else
            {
                baseMod.DisableMod(_localStats, false);
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
                baseMod.DisableMod(_localStats, false);
            }
        }
        
        private void EnableSlot()
        {
            isActive = true;
            
            if (baseMod != null)
            {
                baseMod.EnableMod(_localStats);
            }
        }
    }
}
