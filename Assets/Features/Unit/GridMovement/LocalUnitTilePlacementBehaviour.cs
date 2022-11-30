using Features.GlobalReferences;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Features.Unit.GridMovement
{
    public class LocalUnitTilePlacementBehaviour : NetworkedUnitTilePlacementBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [SerializeField] private CameraFocus_SO cameraFocus;
        [SerializeField] private CanvasFocus_SO canvasFocus;
        [SerializeField] private GameObject visualObject;
        [SerializeField] private GridFocus_SO gridFocus;

        public void OnPointerDown(PointerEventData eventData)
        {
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            gridFocus.Set(Instantiate(visualObject));
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (canvasFocus.Get() == null) return;

            if (cameraFocus.NotNull() && gridFocus.isFixedToMousePosition)
            {
                Vector3 screenPoint = Input.mousePosition;
                screenPoint.z = 10f;
                gridFocus.Get().transform.position = cameraFocus.Get().ScreenToWorldPoint(screenPoint);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Destroy(gridFocus.Get());
            gridFocus.Restore();
        }
    }
}
