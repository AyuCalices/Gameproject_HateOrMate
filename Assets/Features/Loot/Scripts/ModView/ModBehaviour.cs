using Features.Loot.Scripts.GeneratedLoot;
using Features.Unit.Scripts.Behaviours.Mod;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Features.Loot.Scripts.ModView
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ModBehaviour : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private DragControllerFocus_SO dragControllerFocus;
        [SerializeField] private CanvasFocus_SO canvasFocus;
        [SerializeField] private GameObject blankedModPrefab;
        
        [SerializeField] private Image image;
        
        private CanvasGroup _canvasGroup;
        private RectTransform _rectTransform;
        private ExpandBehaviour _expandBehaviour;

        public BaseMod BaseMod { get; private set; }
        public IModContainer CurrentModSlotBehaviour { get; private set; }
        
        private Vector3 _originPosition;

        public bool isInHand;
        private Transform _originTransform;
        private Transform _dragTransform;
        
        private Canvas _tempCanvas;
        private GraphicRaycaster _tempRaycaster;
        private GameObject _hoverGapObject;

        public void SetNewOrigin(IModContainer targetOrigin)
        {
            CurrentModSlotBehaviour = targetOrigin;

            _originTransform = targetOrigin.Transform;
            transform.position = _originTransform.position;
            transform.SetParent(_originTransform);
        }

        public void UpdateColor(Color newColor)
        {
            image.color = newColor;
        }

        public void Initialize(BaseMod baseMod, Transform dragTransform, IModContainer currentModContainer)
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _rectTransform = GetComponent<RectTransform>();
            _expandBehaviour = GetComponent<ExpandBehaviour>();
            BaseMod = baseMod;

            CurrentModSlotBehaviour = currentModContainer;
            _originTransform = transform.parent;
            _dragTransform = dragTransform;
            isInHand = true;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (BaseMod == null)
            {
                Debug.LogError("There is no Mod!");
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            dragControllerFocus.Set(new DragController());

            if (isInHand)
            {
                _expandBehaviour.IsActive = true;
            }
            
            transform.SetParent(_dragTransform);
            _canvasGroup.alpha = 0.5f;
            _canvasGroup.blocksRaycasts = false;
            _originPosition = transform.position;
        }
    
        public void OnDrag(PointerEventData eventData)
        {
            if (canvasFocus.Get() == null) return;
            
            _rectTransform.anchoredPosition += eventData.delta / canvasFocus.Get().scaleFactor;

            Physics.Raycast(Input.mousePosition, new Vector3(0, 0, 1));
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;

            if (!dragControllerFocus.Get().IsSuccessful)
            {
                if (isInHand)
                {
                    _expandBehaviour.SetExpanded(true);
                    _expandBehaviour.IsActive = false;
                }
                transform.position = _originPosition;
            }
            
            transform.SetParent(_originTransform);
            dragControllerFocus.Restore();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Transform parent = transform.parent;
            int siblingIndex = transform.GetSiblingIndex();
            if (parent.childCount - 1 != siblingIndex)
            {
                _hoverGapObject = Instantiate(blankedModPrefab, parent);
                RectTransform rectTransform = (RectTransform) _hoverGapObject.transform;
                Vector2 sizeDelta = rectTransform.sizeDelta;
                rectTransform.sizeDelta = new Vector2(Mathf.Abs(parent.GetComponent<HorizontalLayoutGroup>().spacing) * 2, sizeDelta.y);
                _hoverGapObject.transform.SetSiblingIndex(siblingIndex + 1);
            }

            _tempCanvas = gameObject.AddComponent<Canvas>();
            _tempCanvas.pixelPerfect = false;
            _tempCanvas.overrideSorting = true;
            _tempCanvas.sortingOrder = 22;
            _tempRaycaster = gameObject.AddComponent<GraphicRaycaster>();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Destroy(_tempRaycaster);
            Destroy(_tempCanvas);
            Destroy(_hoverGapObject);
        }
    }
}
