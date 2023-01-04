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
            if (GetComponent<Canvas>() == null)
            {
                _hoverTempCanvas = gameObject.AddComponent<Canvas>();
                _hoverTempCanvas.overrideSorting = true;
                _hoverTempCanvas.sortingOrder = sortingOrder;
            }

            if (GetComponent<GraphicRaycaster>() == null)
            {
                _hoverTempRaycaster = gameObject.AddComponent<GraphicRaycaster>();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            CleanupOverrideSortingOrderOnHover();
        }

        private void CleanupOverrideSortingOrderOnHover()
        {
            if (_hoverTempRaycaster != null)
            {
                Destroy(_hoverTempRaycaster);
            }

            if (_hoverTempCanvas != null)
            {
                Destroy(_hoverTempCanvas);
            }
        }
    }
}
