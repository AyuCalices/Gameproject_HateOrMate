using System;
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
        public static Func<string, UnitClassData_SO, BaseStats, NetworkedBattleBehaviour> onAddUnit;
        public static Action<string, int> onRemoveUnit;
        
        private readonly UnitClassData_SO _classData;
        private readonly ModUnitRuntimeSet_SO _modUnitRuntimeSet;
        private NetworkedBattleBehaviour _instantiatedUnit;
        private int _slot;

        private NetworkedStatsBehaviour _currentUnit;

        public UnitMod(UnitClassData_SO classData, ModUnitRuntimeSet_SO modUnitRuntimeSet, string modName, string description, ModBehaviour modBehaviourPrefab) 
            : base(modName, description, modBehaviourPrefab)
        {
            _classData = classData;
            _modUnitRuntimeSet = modUnitRuntimeSet;
        }

        public override void ApplyToInstantiatedUnit(NetworkedStatsBehaviour instantiatedUnit)
        {
            if (_instantiatedUnit == null) return;
            
            if (_instantiatedUnit.gameObject.GetInstanceID() == instantiatedUnit.gameObject.GetInstanceID()) return;
            
            if (instantiatedUnit.TryGetComponent(out ModUnitBehaviour modUnitBehaviour))
            {
                modUnitBehaviour.UnitMods.ToggleSlot(_slot);
            }
        }

        public override void AddOnSwapSlot(NetworkedStatsBehaviour moddedLocalStats, int slot)
        {
            AddBlockedSlots(moddedLocalStats, slot);
        }

        public override void RemoveOnSwapSlot(NetworkedStatsBehaviour moddedLocalStats)
        {
            RemoveBlockedSlots();
        }

        protected override void InternalAddMod(NetworkedStatsBehaviour moddedLocalStats, int slot)
        {
            _instantiatedUnit = onAddUnit.Invoke("Player", _classData, new BaseStats(10, 50, 3));

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
                
                modUnitBehaviour.UnitMods.ToggleSlot(slot);
            }
        }

        private void RemoveBlockedSlots()
        {
            var list = _modUnitRuntimeSet.GetItems();
            for (int index = list.Count - 1; index >= 0; index--)
            {
                ModUnitBehaviour modUnitBehaviour = list[index];
                if (_currentUnit == null) return;
                if (modUnitBehaviour.gameObject.GetInstanceID() == _currentUnit.gameObject.GetInstanceID()) continue;

                modUnitBehaviour.UnitMods.ToggleSlot(_slot);
            }

            _currentUnit = null;
        }
    }
}
