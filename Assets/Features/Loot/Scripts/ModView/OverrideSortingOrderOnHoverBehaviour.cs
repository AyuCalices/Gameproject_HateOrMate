using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//modding - unit mods cant be moved
//android hover
//multiplayer balancing aspect for grading
//balancing ideas

//save system aufeinander aufbauende daten

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
            if (_hoverTempCanvas == null)
            {
                _hoverTempCanvas = gameObject.AddComponent<Canvas>();
                _hoverTempCanvas.overrideSorting = true;
                _hoverTempCanvas.sortingOrder = sortingOrder;
            }

            if (_hoverTempRaycaster == null)
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
                _hoverTempRaycaster = null;
            }

            if (_hoverTempCanvas != null)
            {
                Destroy(_hoverTempCanvas);
                _hoverTempCanvas = null;
            }
        }
    }
}
