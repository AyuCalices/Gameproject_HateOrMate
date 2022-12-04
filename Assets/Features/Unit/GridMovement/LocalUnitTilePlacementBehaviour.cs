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
            gridRuntimeDictionary.SetAllOrderInLayer(1, 2);
            gridFocus.Set(Instantiate(visualObject));
        }

        public void OnDrag(PointerEventData eventData)
        {
            //TODO: when a unit dies while dragging these events get lost => dont make player be able to drag while battle
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
            gridRuntimeDictionary.SetAllOrderInLayer(-2, -1);
            gridFocus.Restore();
        }
    }
}
