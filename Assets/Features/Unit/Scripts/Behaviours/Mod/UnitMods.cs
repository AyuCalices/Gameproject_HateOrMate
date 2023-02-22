using System;
using Features.Loot.Scripts;
using Features.Loot.Scripts.ModView;

namespace Features.Unit.Scripts.Behaviours.Mod
{
    public class UnitMods
    {
        public static Action<ModBehaviour> onDestroyUnit;
        
        private readonly ModSlotBehaviour[] _modSlotBehaviours;

        public UnitMods(NetworkedStatsBehaviour localStats, ModSlotBehaviour[] modSlotBehaviours)
        {
            _modSlotBehaviours = modSlotBehaviours;
            
            for (int i = 0; i < modSlotBehaviours.Length; i++)
            {
                modSlotBehaviours[i].Initialize(localStats, i);
            }
        }

        public void ApplyToInstantiatedUnit(NetworkedStatsBehaviour instantiatedUnit)
        {
            foreach (ModSlotBehaviour modSlotBehaviour in _modSlotBehaviours)
            {
                modSlotBehaviour.ApplyToInstantiatedUnit(instantiatedUnit);
            }
        }
        
        public void ApplyOnUnitViewInstantiated(UnitViewBehaviour instantiatedView)
        {
            foreach (ModSlotBehaviour modSlotBehaviour in _modSlotBehaviours)
            {
                modSlotBehaviour.ApplyOnUnitViewInstantiated(instantiatedView);
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
                ModSlotBehaviour modSlotBehaviour = _modSlotBehaviours[index];

                if (modSlotBehaviour.ContainsMod)
                {
                    ModBehaviour modBehaviour = modSlotBehaviour.ContainedModBehaviour;
                    modSlotBehaviour.RemoveMod(modBehaviour);
                    onDestroyUnit?.Invoke(modBehaviour);
                }
            }
        }
    }
}
