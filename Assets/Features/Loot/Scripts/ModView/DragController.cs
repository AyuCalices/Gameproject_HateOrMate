using Features.Loot.Scripts.GeneratedLoot;
using Features.Unit.Scripts.Behaviours.Mod;
using UnityEngine;

namespace Features.Loot.Scripts.ModView
{
    public class DragController
    {
        private readonly ModDragBehaviour _originModDragBehaviour;
        private readonly BaseMod _originMod;
        private readonly ModSlotContainer _originModSlotContainer;
        private readonly ModSlotBehaviour _originModSlotBehaviour;

        public bool IsSuccessful { get; private set; }

        public DragController(ModDragBehaviour originModDragBehaviour, BaseMod originMod, ModSlotContainer originModSlotContainer, ModSlotBehaviour originModSlotBehaviour)
        {
            _originModDragBehaviour = originModDragBehaviour;
            _originMod = originMod;
            _originModSlotContainer = originModSlotContainer;
            _originModSlotBehaviour = originModSlotBehaviour;
            IsSuccessful = false;
        }

        public void AddOrExchangeMod(ModSlotContainer targetModSlotContainer, ModDragBehaviour targetModDragBehaviour, ModSlotBehaviour targetModSlotBehaviour)
        {
            targetModSlotContainer.AddOrExchangeMod(_originMod, _originModSlotContainer);
            IsSuccessful = true;
        
            if (targetModDragBehaviour == null)
            {
                Debug.Log("whowiejfoiewjf");
                if (_originModSlotBehaviour != null)
                {
                    _originModSlotBehaviour.ContainedModDragBehaviour = null;
                }
                _originModDragBehaviour.SetNewOrigin(targetModSlotContainer, targetModSlotBehaviour);
            }
            else
            {
                targetModDragBehaviour.SetNewOrigin(_originModSlotContainer, _originModSlotBehaviour);
                _originModDragBehaviour.SetNewOrigin(targetModSlotContainer, targetModSlotBehaviour);
            }
        }
    }
}