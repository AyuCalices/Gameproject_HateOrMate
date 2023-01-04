using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Features.Loot.Scripts.ModView
{
    public class OverrideSortingOrderOnHoverBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private int sortingOrder = 22;
    
        private Canvas _hoverTempCanvas;
        private GraphicRaycaster _hoverTempRaycaster;
    
        public void OnPointerEnter(PointerEventData eventData)
        {
            OverrideSortingOrderOnHover();
        }

        private void OverrideSortingOrderOnHover()
        {
            _hoverTempCanvas = gameObject.AddComponent<Canvas>();
            _hoverTempCanvas.overrideSorting = true;
            _hoverTempCanvas.sortingOrder = sortingOrder;
            _hoverTempRaycaster = gameObject.AddComponent<GraphicRaycaster>();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            CleanupOverrideSortingOrderOnHover();
        }

        private void CleanupOverrideSortingOrderOnHover()
        {
            Destroy(_hoverTempRaycaster);
            Destroy(_hoverTempCanvas);
        }
    }
}
