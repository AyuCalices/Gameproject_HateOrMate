using System.Collections.Generic;
using Features.Loot.Scripts.GeneratedLoot;
using Features.Loot.Scripts.ModView;
using Features.Unit.Scripts.Behaviours.Stat;
using UnityEngine;

namespace Features.Unit.Scripts.Behaviours.Mod
{
    public class UnitMods
    {
        private readonly ModSlotContainer[] _modSlotsContainers;
        private readonly List<ModSlotBehaviour> _modSlotBehaviours;

        public UnitMods(int size, NetworkedStatsBehaviour localStats, List<ModSlotBehaviour> modSlotBehaviours)
        {
            this._modSlotBehaviours = modSlotBehaviours;
            
            _modSlotsContainers = new ModSlotContainer[size];
            for (int i = 0; i < size; i++)
            {
                _modSlotsContainers[i] = new ModSlotContainer(localStats);
                modSlotBehaviours[i].Init(_modSlotsContainers[i]);

                if (i > 2)
                {
                    _modSlotsContainers[i].DisableSlot();
                }
            }
        }
        
        public bool TryInstantiateMod(ModDragBehaviour modDragBehaviourPrefab, BaseMod baseMod)
        {
            for (int index = 0; index < _modSlotsContainers.Length; index++)
            {
                ModSlotContainer modSlotsContainer = _modSlotsContainers[index];
                if (!modSlotsContainer.ContainsMod())
                {
                    ModDragBehaviour modDragBehaviour = Object.Instantiate(modDragBehaviourPrefab, _modSlotBehaviours[index].transform);
                    modDragBehaviour.Initialize(modSlotsContainer, _modSlotBehaviours[index], baseMod);
                    modSlotsContainer.AddMod(baseMod);
                    return true;
                }
            }

            return false;
        }

        public void AddModToInstantiatedUnit(NetworkedStatsBehaviour instantiatedUnit)
        {
            foreach (ModSlotContainer modSlotContainer in _modSlotsContainers)
            {
                modSlotContainer.ApplyModToInstantiatedUnit(instantiatedUnit);
            }
        }

        public bool TryAddMod(ModDragBehaviour modDragBehaviour)
        {
            for (int index = 0; index < _modSlotsContainers.Length; index++)
            {
                ModSlotContainer modSlotsContainer = _modSlotsContainers[index];
                if (!modSlotsContainer.ContainsMod())
                {
                    modSlotsContainer.AddMod(modDragBehaviour.BaseMod);
                    modDragBehaviour.SetNewOrigin(modSlotsContainer, _modSlotBehaviours[index]);

                    return true;
                }
            }

            return false;
        }
        
        public void ToggleSlot(int index)
        {
            _modSlotsContainers[index].ToggleSlot();
            _modSlotBehaviours[index].UpdateSlot();
        }
    }

    public class ModSlotContainer
    {
        private BaseMod _baseMod;
        private bool _isActive;
        private readonly NetworkedStatsBehaviour _localStats;

        public ModSlotContainer(NetworkedStatsBehaviour localStats)
        {
            this._localStats = localStats;
            _isActive = true;
        }

        public void ApplyModToInstantiatedUnit(NetworkedStatsBehaviour instantiatedUnit)
        {
            if (!ContainsMod()) return;
            if (!_isActive) return;
            
            _baseMod.ApplyToInstantiatedUnit(instantiatedUnit);
        }

        public bool ContainsMod()
        {
            return _baseMod != null;
        }

        public void AddOrExchangeMod(BaseMod newMod, ModSlotContainer origin)
        {
            BaseMod removedMod = _baseMod;
            
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
            if (!_isActive)
            {
                newMod.DisableMod(_localStats, false);
                origin._baseMod = null;
            }
            else if (_isActive)
            {
                AddMod(newMod);
            }
        }

        public void AddMod(BaseMod newMod)
        {
            _baseMod = newMod;

            if (_isActive) newMod.EnableMod(_localStats);
        }
        
        private void RemoveMod(bool isSwap)
        {
            if (_isActive) _baseMod.DisableMod(_localStats, isSwap);
            
            _baseMod = null;
        }

        public void UpdateActiveStatus()
        {
            if (_isActive)
            {
                _baseMod.EnableMod(_localStats);
            }
            else
            {
                _baseMod.DisableMod(_localStats, false);
            }
        }
        
        public void ToggleSlot()
        {
            if (_isActive)
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
            _isActive = false;

            if (_baseMod != null)
            {
                _baseMod.DisableMod(_localStats, false);
            }
        }
        
        private void EnableSlot()
        {
            _isActive = true;
            
            if (_baseMod != null)
            {
                _baseMod.EnableMod(_localStats);
            }
        }
    }
}
