using Features.Mod;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Features.ModHUD
{
    [RequireComponent(typeof(CanvasGroup))]
    public class DragHandler : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [SerializeField] private BaseModGenerator_SO baseModGenerator;
        [SerializeField] private Canvas canvas;
        
        private CanvasGroup _canvasGroup;
        private RectTransform _rectTransform;
        private Vector3 _startPosition;

        public BaseMod BaseMod { get; private set; }

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _rectTransform = GetComponent<RectTransform>();

            BaseMod = baseModGenerator.Generate();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _startPosition = transform.position;
            _canvasGroup.alpha = 0.5f;
            _canvasGroup.blocksRaycasts = false;
        }
    
        public void OnDrag(PointerEventData eventData)
        {
            _rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

            Physics.Raycast(Input.mousePosition, new Vector3(0, 0, 1));
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;
        }
    }
}
