using UnityEngine;
using UnityEngine.EventSystems;

namespace Features.Mods.Scripts.View.ModContainer
{
    public class DestroyModContainerBehaviour : MonoBehaviour, IDropHandler
    {
        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null) return;
            eventData.pointerDrag.TryGetComponent(out ModViewBehaviour movingMod);
            if (movingMod == null) return;
        
            movingMod.CurrentModContainer.RemoveMod(movingMod);
            Destroy(movingMod.gameObject);
        }
    }
}
