using System;
using Features.Battle.Scripts.Unit.ServiceLocatorSystem;
using Features.Connection.Scripts;
using Features.Loot.Scripts.ModView;
using Features.Unit.Scripts;
using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Behaviours.Mod;
using Photon.Pun;
using UnityEngine;

namespace Features.Loot.Scripts.GeneratedLoot
{
    public class UnitMod : BaseMod
    {
        public static Func<string, UnitClassData_SO, int, NetworkedBattleBehaviour> onAddUnit;
        
        private readonly UnitClassData_SO _classData;
        private readonly UnitViewRuntimeSet_SO _unitViewRuntimeSet;
        private NetworkedBattleBehaviour _instantiatedUnit;
        private int _slot;

        private UnitServiceProvider _currentUnit;

        public UnitMod(UnitClassData_SO classData, UnitViewRuntimeSet_SO unitViewRuntimeSet, GameObject spritePrefab, string description, int level, ModBehaviour modBehaviourPrefab) 
            : base(spritePrefab, description, level, modBehaviourPrefab)
        {
            _classData = classData;
            _unitViewRuntimeSet = unitViewRuntimeSet;
            _slot = -1;
        }

        public override bool IsValidAddMod(UnitServiceProvider requestedUnitServiceProvider, int slot, ErrorPopup errorPopup, Transform transform)
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
        
        protected override void InternalAddMod(UnitServiceProvider modifiedUnitServiceProvider, int slot)
        {
            AddBlockedSlots(modifiedUnitServiceProvider, slot);
            
            _instantiatedUnit = onAddUnit.Invoke("Player", _classData, Level);
        }

        protected override void InternalRemoveMod(UnitServiceProvider modifiedUnitServiceProvider)
        {
            PhotonNetwork.Destroy(_instantiatedUnit.gameObject);
            
            RemoveBlockedSlots();
        }
        
        private void AddBlockedSlots(UnitServiceProvider modifiedUnitServiceProvider, int slot)
        {
            _slot = slot;
            _currentUnit = modifiedUnitServiceProvider;
            foreach (UnitViewBehaviour unitViewBehaviour in _unitViewRuntimeSet.GetItems())
            {
                if (unitViewBehaviour.UnitServiceProvider.gameObject.GetInstanceID() == modifiedUnitServiceProvider.gameObject.GetInstanceID()) continue;
                
                unitViewBehaviour.UnitMods.DisableSlot(slot);
            }
        }

        private void RemoveBlockedSlots()
        {
            var list = _unitViewRuntimeSet.GetItems();
            foreach (UnitViewBehaviour unitViewBehaviour in list)
            {
                if (_currentUnit == null) return;
                if (unitViewBehaviour.UnitServiceProvider.gameObject.GetInstanceID() == _currentUnit.gameObject.GetInstanceID()) continue;

                unitViewBehaviour.UnitMods.EnableSlot(_slot);
            }

            _slot = -1;
            _currentUnit = null;
        }
    }
}
