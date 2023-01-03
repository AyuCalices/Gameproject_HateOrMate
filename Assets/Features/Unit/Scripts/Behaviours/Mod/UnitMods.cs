using Features.Loot.Scripts.GeneratedLoot;
using Features.Loot.Scripts.ModView;
using Features.Unit.Scripts.Behaviours.Stat;
using UnityEngine;

namespace Features.Unit.Scripts.Behaviours.Mod
{
    public class UnitMods
    {
        private readonly ModSlotContainer[] _modSlotsContainers;
        private readonly ModSlotBehaviour[] _modSlotBehaviours;

        public UnitMods(NetworkedStatsBehaviour localStats, ModSlotBehaviour[] modSlotBehaviours)
        {
            _modSlotBehaviours = modSlotBehaviours;
            
            _modSlotsContainers = new ModSlotContainer[modSlotBehaviours.Length];
            for (int i = 0; i < modSlotBehaviours.Length; i++)
            {
                _modSlotsContainers[i] = new ModSlotContainer(localStats);
                modSlotBehaviours[i].Init(_modSlotsContainers[i]);

                if (i > 2)
                {
                    ToggleSlot(i);
                    //_modSlotsContainers[i].DisableSlot();
                }
            }
        }

        public void AddModToInstantiatedUnit(NetworkedStatsBehaviour instantiatedUnit)
        {
            foreach (ModSlotContainer modSlotContainer in _modSlotsContainers)
            {
                modSlotContainer.ApplyModToInstantiatedUnit(instantiatedUnit);
            }
        }
        
        //TODO: one call station
        public void ToggleSlot(int index)
        {
            _modSlotsContainers[index].ToggleSlot();
            _modSlotBehaviours[index].UpdateSlot();
        }

        public void OnDestroy()
        {
            for (int index = _modSlotBehaviours.Length - 1; index >= 0; index--)
            {
                ModSlotBehaviour modSlotBehaviour = _modSlotBehaviours[index];
                ModSlotContainer modSlotsContainer = _modSlotsContainers[index];

                Object.Destroy(modSlotBehaviour);
                modSlotsContainer.RemoveMod(false);
            }
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

        private bool ContainsMod()
        {
            return _baseMod != null;
        }

        //TODO: not clear enough what is target and what origin - use abstraction to differentiate between hand and slot
        public void AddOrExchangeMod(BaseMod newMod, ModSlotContainer origin)
        {
            if (this == origin) return;
            
            BaseMod removedMod = _baseMod;
            
            //can be null due to hand
            if (ContainsMod())
            {
                RemoveMod(true);
            }

            //can be null due to hand
            if (origin != null)
            {
                //can be null due to hand
                if (origin.ContainsMod())
                {
                    origin.RemoveMod(true);
                }

                //can be null due to hand
                if (removedMod != null)
                {
                    origin.SwapAddMod(removedMod, origin);
                }
            }

            SwapAddMod(newMod, this);
        }

        private void SwapAddMod(BaseMod newMod, ModSlotContainer target)
        {
            if (target == null) return;
            
            if (!target._isActive)
            {
                newMod.DisableMod(target._localStats, false);
            }
            else if (target._isActive)
            {
                target.AddMod(newMod);
            }
        }

        private void AddMod(BaseMod newMod)
        {
            _baseMod = newMod;

            if (_isActive) newMod.EnableMod(_localStats);
        }
        
        public void RemoveMod(bool isSwap)
        {
            if (_isActive && _baseMod != null) _baseMod.DisableMod(_localStats, isSwap);
            
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
