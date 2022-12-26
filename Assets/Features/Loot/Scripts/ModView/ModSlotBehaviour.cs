using Features.Unit.Scripts.Behaviours.Mod;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Features.Loot.Scripts.ModView
{
    public class ModSlotBehaviour : MonoBehaviour, IDropHandler
    {
        [SerializeField] private DragControllerFocus_SO dragControllerFocus;

        private ModSlotContainer _modSlotContainer;

        public ModDragBehaviour ContainedModDragBehaviour { get; set; }

        public void Init(ModSlotContainer modSlotContainer)
        {
            _modSlotContainer = modSlotContainer;
        }

        public void UpdateSlot()
        {
            //TODO: implement UI
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null) return;
            eventData.pointerDrag.TryGetComponent(out ModDragBehaviour movingMod);
            if (movingMod == null) return;

            dragControllerFocus.Get().AddOrExchangeMod(_modSlotContainer, ContainedModDragBehaviour, this);
        }
    }
}
