using System;
using Features.Loot.Scripts;
using Features.Loot.Scripts.ModView;
using Features.Unit.Scripts.Behaviours.Battle;

namespace Features.Unit.Scripts.Behaviours.Mod
{
    public class UnitDisplayMods
    {
        public static Action<ModBehaviour> onDestroyUnit;
        
        private readonly UnitModContainerBehaviour[] _modSlotBehaviours;

        public UnitDisplayMods(UnitServiceProvider instantiatedUnitServiceProvider, UnitModContainerBehaviour[] modSlotBehaviours)
        {
            _modSlotBehaviours = modSlotBehaviours;
            
            for (int i = 0; i < modSlotBehaviours.Length; i++)
            {
                modSlotBehaviours[i].Initialize(instantiatedUnitServiceProvider, i);
            }
        }

        public void ApplyToInstantiatedUnit(UnitServiceProvider instantiatedUnitServiceProvider)
        {
            foreach (UnitModContainerBehaviour modSlotBehaviour in _modSlotBehaviours)
            {
                modSlotBehaviour.ApplyToInstantiatedUnit(instantiatedUnitServiceProvider);
            }
        }
        
        public void ApplyOnUnitViewInstantiated(UnitDisplayBehaviour unitDisplayBehaviour)
        {
            foreach (UnitModContainerBehaviour modSlotBehaviour in _modSlotBehaviours)
            {
                modSlotBehaviour.ApplyOnUnitViewInstantiated(unitDisplayBehaviour);
            }
        }

        public bool SlotIsEnabled(int index)
        {
            return _modSlotBehaviours[index].IsActive;
        }

        public void DisableSlot(int index)
        {
            _modSlotBehaviours[index].DisableSlot();
        }
        
        public void EnableSlot(int index)
        {
            _modSlotBehaviours[index].EnableSlot();
        }

        public void OnDestroy()
        {
            for (int index = _modSlotBehaviours.Length - 1; index >= 0; index--)
            {
                UnitModContainerBehaviour unitModContainerBehaviour = _modSlotBehaviours[index];

                if (unitModContainerBehaviour.ContainsMod)
                {
                    ModBehaviour modBehaviour = unitModContainerBehaviour.ContainedModBehaviour;
                    unitModContainerBehaviour.RemoveMod(modBehaviour);
                    onDestroyUnit?.Invoke(modBehaviour);
                }
            }
        }
    }
}
