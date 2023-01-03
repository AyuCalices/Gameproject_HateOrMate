using Features.Loot.Scripts.GeneratedLoot;
using Features.Unit.Scripts.Behaviours.Mod;
using UnityEngine;

namespace Features.Loot.Scripts.ModView
{
    public class DragController
    {
        private readonly ModDragBehaviour _originModDragBehaviour;
        private readonly ModSlotBehaviour _originModSlotBehaviour;

        public bool IsSuccessful { get; private set; }

        public DragController(ModDragBehaviour originModDragBehaviour, ModSlotBehaviour originModSlotBehaviour)
        {
            _originModDragBehaviour = originModDragBehaviour;
            _originModSlotBehaviour = originModSlotBehaviour;
            IsSuccessful = false;
        }

        public void AddOrExchangeMod(ModDragBehaviour targetModDragBehaviour, ModSlotBehaviour targetModSlotBehaviour)
        {
            targetModSlotBehaviour.AddOrExchangeMod(_originModDragBehaviour, _originModSlotBehaviour);
            IsSuccessful = true;
        
            if (targetModDragBehaviour == null)     //no swap
            {
                if (_originModSlotBehaviour != null)
                {
                    _originModSlotBehaviour.ContainedModDragBehaviour = null;
                }
                _originModDragBehaviour.SetNewOrigin(targetModSlotBehaviour);
            }
            else        //swap
            {
                targetModDragBehaviour.SetNewOrigin(_originModSlotBehaviour);
                _originModDragBehaviour.SetNewOrigin(targetModSlotBehaviour);
            }
        }
    }
}