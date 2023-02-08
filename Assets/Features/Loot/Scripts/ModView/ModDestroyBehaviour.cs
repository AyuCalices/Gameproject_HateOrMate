using UnityEngine;
using UnityEngine.EventSystems;

namespace Features.Loot.Scripts.ModView
{
    public class ModDestroyBehaviour : MonoBehaviour, IDropHandler
    {
        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null) return;
            eventData.pointerDrag.TryGetComponent(out ModBehaviour movingMod);
            if (movingMod == null) return;
        
            movingMod.CurrentModContainer.RemoveMod(movingMod);
            Destroy(movingMod.gameObject);
        }
    }
}
