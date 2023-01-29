using System;
using Features.Connection.Scripts;
using Features.Loot.Scripts.ModView;
using Features.Unit.Scripts;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Behaviours.Mod;
using Features.Unit.Scripts.Behaviours.Stat;
using UnityEngine;

namespace Features.Loot.Scripts.GeneratedLoot
{
    public class UnitMod : BaseMod
    {
        public static Func<string, UnitClassData_SO, NetworkedBattleBehaviour> onAddUnit;
        public static Action<string, int> onRemoveUnit;
        
        private readonly UnitClassData_SO _classData;
        private readonly ModUnitRuntimeSet_SO _modUnitRuntimeSet;
        private NetworkedBattleBehaviour _instantiatedUnit;
        private int _slot;

        private NetworkedStatsBehaviour _currentUnit;

        public UnitMod(UnitClassData_SO classData, ModUnitRuntimeSet_SO modUnitRuntimeSet, GameObject spritePrefab, string description, int level, ModBehaviour modBehaviourPrefab) 
            : base(spritePrefab, description, level, modBehaviourPrefab)
        {
            _classData = classData;
            _modUnitRuntimeSet = modUnitRuntimeSet;
        }

        public override bool IsValidAddMod(NetworkedStatsBehaviour instantiatedUnit, int slot, ErrorPopup errorPopup, Transform transform)
        {
            foreach (ModUnitBehaviour modUnitBehaviour in _modUnitRuntimeSet.GetItems())
            {
                if (!modUnitBehaviour.UnitMods.SlotIsEnabled(slot))
                {
                    errorPopup.Instantiate(transform, "Unit Mod cant be added on a locked slot!");
                    return false;
                }
            }

            if (_instantiatedUnit != null)
            {
                errorPopup.Instantiate(transform, "Unit Mod cant be Moved!");
            }
            return _instantiatedUnit == null;
        }

        public override void ApplyToInstantiatedUnit(NetworkedStatsBehaviour instantiatedUnit)
        {
            if (_instantiatedUnit == null) return;
            
            if (_instantiatedUnit.gameObject.GetInstanceID() == instantiatedUnit.gameObject.GetInstanceID()) return;
            
            if (instantiatedUnit.TryGetComponent(out ModUnitBehaviour modUnitBehaviour))
            {
                modUnitBehaviour.UnitMods.DisableSlot(_slot);
            }
        }

        protected override void InternalAddMod(NetworkedStatsBehaviour moddedLocalStats, int slot)
        {
            _instantiatedUnit = onAddUnit.Invoke("Player", _classData);

            AddBlockedSlots(moddedLocalStats, slot);
        }

        protected override void InternalRemoveMod(NetworkedStatsBehaviour moddedLocalStats)
        {
            if (_instantiatedUnit != null)
            {
                onRemoveUnit.Invoke("Player", _instantiatedUnit.PhotonView.ViewID);
                _instantiatedUnit = null;
                RemoveBlockedSlots();
            }
        }

        private void AddBlockedSlots(NetworkedStatsBehaviour moddedLocalStats, int slot)
        {
            _slot = slot;
            _currentUnit = moddedLocalStats;
            foreach (ModUnitBehaviour modUnitBehaviour in _modUnitRuntimeSet.GetItems())
            {
                if (modUnitBehaviour.gameObject.GetInstanceID() == moddedLocalStats.gameObject.GetInstanceID()) continue;
                
                modUnitBehaviour.UnitMods.DisableSlot(slot);
            }
        }

        private void RemoveBlockedSlots()
        {
            var list = _modUnitRuntimeSet.GetItems();
            foreach (ModUnitBehaviour modUnitBehaviour in list)
            {
                if (_currentUnit == null) return;
                if (modUnitBehaviour.gameObject.GetInstanceID() == _currentUnit.gameObject.GetInstanceID()) continue;

                modUnitBehaviour.UnitMods.EnableSlot(_slot);
            }

            _currentUnit = null;
        }
    }
}
