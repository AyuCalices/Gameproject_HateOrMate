using Features.ModView;
using Features.Unit;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Features.Grid
{
    public class LocalUnitDragBehaviour : NetworkedUnitDragBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [SerializeField] private CanvasFocus_SO canvasFocus;
        [SerializeField] private GameObject visualObject;
        [SerializeField] private GameObjectFocus_SO gameObjectFocus;

        public void OnPointerDown(PointerEventData eventData)
        {
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            gameObjectFocus.Set(Instantiate(visualObject));
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (canvasFocus.Get() == null) return;

            Camera mainCamera = Camera.main;
            if (mainCamera != null && gameObjectFocus.isFixedToMousePosition)
            {
                Vector3 screenPoint = Input.mousePosition;
                screenPoint.z = 10f;
                gameObjectFocus.Get().transform.position = mainCamera.ScreenToWorldPoint(screenPoint);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Destroy(gameObjectFocus.Get());
            gameObjectFocus.Restore();
        }
    }
}
