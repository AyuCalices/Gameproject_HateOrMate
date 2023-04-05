using System;
using Features.Mods.Scripts.View.ModContainer;
using Features.Unit.Scripts.Behaviours;
using UnityEngine;

namespace Features.Mods.Scripts.View
{
    public class UnitDisplayMods
    {
        public static Action<ModViewBehaviour> onDestroyUnit;
        
        private readonly UnitModContainerBehaviour[] _modContainerBehaviours;

        public UnitDisplayMods(UnitServiceProvider instantiatedUnitServiceProvider, UnitModContainerBehaviour[] modContainerBehaviours)
        {
            _modContainerBehaviours = modContainerBehaviours;
            
            for (int i = 0; i < modContainerBehaviours.Length; i++)
            {
                modContainerBehaviours[i].Initialize(instantiatedUnitServiceProvider, i);
            }
        }

        public void ApplyToInstantiatedUnit(UnitServiceProvider instantiatedUnitServiceProvider)
        {
            foreach (UnitModContainerBehaviour modSlotBehaviour in _modContainerBehaviours)
            {
                modSlotBehaviour.ApplyToInstantiatedUnit(instantiatedUnitServiceProvider);
            }
        }
        
        public void ApplyOnUnitViewInstantiated(UnitDisplayBehaviour unitDisplayBehaviour)
        {
            foreach (UnitModContainerBehaviour modSlotBehaviour in _modContainerBehaviours)
            {
                modSlotBehaviour.ApplyOnUnitViewInstantiated(unitDisplayBehaviour);
            }
        }

        public bool SlotIsEnabled(int index)
        {
            return _modContainerBehaviours[index].IsActive;
        }

        public void DisableSlot(int index)
        {
            _modContainerBehaviours[index].DisableSlot();
        }
        
        public void EnableSlot(int index)
        {
            _modContainerBehaviours[index].EnableSlot();
        }

        public void OnDestroy()
        {
            for (int index = _modContainerBehaviours.Length - 1; index >= 0; index--)
            {
                UnitModContainerBehaviour unitModContainerBehaviour = _modContainerBehaviours[index];

                if (unitModContainerBehaviour.ContainsMod)
                {
                    ModViewBehaviour modViewBehaviour = unitModContainerBehaviour.ContainedModViewBehaviour;
                    unitModContainerBehaviour.RemoveMod(modViewBehaviour);
                    onDestroyUnit?.Invoke(modViewBehaviour);
                }
            }
        }
    }
}
