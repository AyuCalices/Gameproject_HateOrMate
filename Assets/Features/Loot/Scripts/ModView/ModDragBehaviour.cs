using ExitGames.Client.Photon.StructWrapping;
using Features.Loot.Scripts.GeneratedLoot;
using Features.Unit.Scripts.Behaviours.Mod;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Features.Loot.Scripts.ModView
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ModDragBehaviour : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private DragControllerFocus_SO dragControllerFocus;
        [SerializeField] private CanvasFocus_SO canvasFocus;
        
        private CanvasGroup _canvasGroup;
        private RectTransform _rectTransform;
        private ExpandBehaviour _expandBehaviour;

        private BaseMod BaseMod { get; set; }
        private ModSlotContainer _modSlotContainer;
        private ModSlotBehaviour _modSlotBehaviour;
        private Vector3 _originPosition;

        private bool _isInHand;
        private Transform _originTransform;
        private Transform _dragTransform;
        
        private Canvas _tempCanvas;
        private GraphicRaycaster _tempRaycaster;

        public void SetNewOrigin(ModSlotContainer targetSlotContainer, ModSlotBehaviour targetOrigin)
        {
            _modSlotContainer = targetSlotContainer;
            _modSlotBehaviour = targetOrigin;
            targetOrigin.ContainedModDragBehaviour = this;

            _originTransform = targetOrigin.transform;
            transform.position = _originTransform.position;
            
            _isInHand = false;
        }

        public void Initialize(BaseMod baseMod, Transform dragTransform)
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _rectTransform = GetComponent<RectTransform>();
            _expandBehaviour = GetComponent<ExpandBehaviour>();
            BaseMod = baseMod;

            _originTransform = transform.parent;
            _dragTransform = dragTransform;
            _isInHand = true;
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
            dragControllerFocus.Set(new DragController(this, BaseMod, _modSlotContainer, _modSlotBehaviour));

            if (_isInHand)
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
                if (_isInHand)
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
        }
    }
}
