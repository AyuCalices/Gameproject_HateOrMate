using System;
using Features.Connection.Scripts;
using Features.Loot.Scripts.ModView;
using Features.Unit.Scripts;
using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Behaviours.Mod;
using UnityEngine;

namespace Features.Loot.Scripts.GeneratedLoot
{
    public class UnitMod : BaseMod
    {
        public static Func<string, UnitClassData_SO, int, NetworkedBattleBehaviour> onAddUnit;
        public static Action<string, int> onRemoveUnit;
        
        private readonly UnitClassData_SO _classData;
        private readonly UnitViewRuntimeSet_SO _unitViewRuntimeSet;
        private NetworkedBattleBehaviour _instantiatedUnit;
        private int _slot;

        private NetworkedStatsBehaviour _currentUnit;

        public UnitMod(UnitClassData_SO classData, UnitViewRuntimeSet_SO unitViewRuntimeSet, GameObject spritePrefab, string description, int level, ModBehaviour modBehaviourPrefab) 
            : base(spritePrefab, description, level, modBehaviourPrefab)
        {
            _classData = classData;
            _unitViewRuntimeSet = unitViewRuntimeSet;
            _slot = -1;
        }

        public override bool IsValidAddMod(NetworkedStatsBehaviour instantiatedUnit, int slot, ErrorPopup errorPopup, Transform transform)
        {
            foreach (UnitViewBehaviour unitViewBehaviour in _unitViewRuntimeSet.GetItems())
            {
                if (!unitViewBehaviour.UnitMods.SlotIsEnabled(slot))
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

        public override void ApplyOnUnitViewInstantiated(UnitViewBehaviour unitViewBehaviour)
        {
            if (_instantiatedUnit == null || _slot == -1) return;

            unitViewBehaviour.UnitMods.DisableSlot(_slot);
        }
        
        protected override void InternalAddMod(NetworkedStatsBehaviour moddedLocalStats, int slot)
        {
            AddBlockedSlots(moddedLocalStats, slot);
            
            _instantiatedUnit = onAddUnit.Invoke("Player", _classData, Level);
        }

        protected override void InternalRemoveMod(NetworkedStatsBehaviour moddedLocalStats)
        {
            onRemoveUnit.Invoke("Player", _instantiatedUnit.PhotonView.ViewID);
            
            RemoveBlockedSlots();
        }
        
        private void AddBlockedSlots(NetworkedStatsBehaviour moddedLocalStats, int slot)
        {
            _slot = slot;
            _currentUnit = moddedLocalStats;
            foreach (UnitViewBehaviour unitViewBehaviour in _unitViewRuntimeSet.GetItems())
            {
                if (unitViewBehaviour.UnitOwnerStats.gameObject.GetInstanceID() == moddedLocalStats.gameObject.GetInstanceID()) continue;
                
                unitViewBehaviour.UnitMods.DisableSlot(slot);
            }
        }

        private void RemoveBlockedSlots()
        {
            var list = _unitViewRuntimeSet.GetItems();
            foreach (UnitViewBehaviour unitViewBehaviour in list)
            {
                if (_currentUnit == null) return;
                if (unitViewBehaviour.UnitOwnerStats.gameObject.GetInstanceID() == _currentUnit.gameObject.GetInstanceID()) continue;

                unitViewBehaviour.UnitMods.EnableSlot(_slot);
            }

            _slot = -1;
            _currentUnit = null;
        }
    }
}
