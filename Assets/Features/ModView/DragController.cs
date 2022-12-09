using DataStructures.Focus;
using Features.Mod;
using Features.Unit;
using Features.Unit.Modding;
using UnityEngine;

namespace Features.ModView
{
    public class DragController
    {
        private ModDragBehaviour _originModDragBehaviour;
        private BaseMod _originMod;
        private ModSlotContainer _originModSlotContainer;
        private ModSlotBehaviour _originModSlotBehaviour;

        public DragController(ModDragBehaviour originModDragBehaviour, BaseMod originMod, ModSlotContainer originModSlotContainer, ModSlotBehaviour originModSlotBehaviour)
        {
            _originModDragBehaviour = originModDragBehaviour;
            _originMod = originMod;
            _originModSlotContainer = originModSlotContainer;
            _originModSlotBehaviour = originModSlotBehaviour;
        }

        public void AddOrExchangeMod(ModSlotContainer targetModSlotContainer, ModDragBehaviour targetModDragBehaviour, ModSlotBehaviour targetModSlotBehaviour)
        {
            targetModSlotContainer.AddOrExchangeMod(_originMod, _originModSlotContainer);
        
            if (targetModDragBehaviour == null)
            {
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