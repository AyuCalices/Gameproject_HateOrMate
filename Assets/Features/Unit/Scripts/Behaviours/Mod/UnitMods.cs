using Features.Loot.Scripts.GeneratedLoot;
using Features.Loot.Scripts.ModView;
using Features.Unit.Scripts.Behaviours.Stat;
using UnityEngine;

namespace Features.Unit.Scripts.Behaviours.Mod
{
    public class UnitMods
    {
        private readonly ModSlotBehaviour[] _modSlotBehaviours;

        public UnitMods(NetworkedStatsBehaviour localStats, ModSlotBehaviour[] modSlotBehaviours)
        {
            _modSlotBehaviours = modSlotBehaviours;
            
            for (int i = 0; i < modSlotBehaviours.Length; i++)
            {
                modSlotBehaviours[i].Init(localStats);

                if (i > 2)
                {
                    ToggleSlot(i);
                    //_modSlotsContainers[i].DisableSlot();
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
        
        //TODO: one call station
        public void ToggleSlot(int index)
        {
            _modSlotBehaviours[index].ToggleSlot();
            _modSlotBehaviours[index].UpdateSlot();
        }

        public void OnDestroy()
        {
            for (int index = _modSlotBehaviours.Length - 1; index >= 0; index--)
            {
                ModSlotBehaviour modSlotBehaviour = _modSlotBehaviours[index];

                Object.Destroy(modSlotBehaviour);
                modSlotBehaviour.RemoveMod(false);
            }
        }
    }
}
