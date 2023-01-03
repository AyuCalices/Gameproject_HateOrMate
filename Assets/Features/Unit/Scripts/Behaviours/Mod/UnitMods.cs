using System;
using Features.Loot.Scripts.ModView;
using Features.Unit.Scripts.Behaviours.Stat;

namespace Features.Unit.Scripts.Behaviours.Mod
{
    public class UnitMods
    {
        public static Action<ModBehaviour> onMoveToHand;
        
        private readonly ModSlotBehaviour[] _modSlotBehaviours;

        public UnitMods(NetworkedStatsBehaviour localStats, ModSlotBehaviour[] modSlotBehaviours)
        {
            _modSlotBehaviours = modSlotBehaviours;
            
            for (int i = 0; i < modSlotBehaviours.Length; i++)
            {
                modSlotBehaviours[i].Init(localStats);

                if (i > 2)
                {
                    _modSlotBehaviours[i].DisableSlot();
                }
            }
        }

        public void AddModToInstantiatedUnit(NetworkedStatsBehaviour instantiatedUnit)
        {
            foreach (ModSlotBehaviour modSlotBehaviour in _modSlotBehaviours)
            {
                modSlotBehaviour.ApplyModToInstantiatedUnit(instantiatedUnit);
            }
        }
        
        public void ToggleSlot(int index)
        {
            _modSlotBehaviours[index].ToggleSlot();
        }

        public void OnDestroy()
        {
            for (int index = _modSlotBehaviours.Length - 1; index >= 0; index--)
            {
                ModSlotBehaviour modSlotBehaviour = _modSlotBehaviours[index];

                if (modSlotBehaviour.ContainsMod())
                {
                    ModBehaviour modBehaviour = modSlotBehaviour.ContainedModBehaviour;
                    onMoveToHand?.Invoke(modBehaviour);
                    modSlotBehaviour.RemoveMod(modBehaviour, false);
                }
            }
        }
    }
}