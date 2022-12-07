using Features.GlobalReferences;
using Features.Loot;
using Features.Mod;
using Features.Unit;
using Features.Unit.Modding;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Features.ModView
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ModDragBehaviour : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [SerializeField] private DragControllerFocus_SO dragControllerFocus;
        [SerializeField] private LootableGenerator_SO lootableGenerator;
        [SerializeField] private CanvasFocus_SO canvasFocus;
        
        private CanvasGroup _canvasGroup;
        private RectTransform _rectTransform;

        public BaseMod BaseMod { get; set; }
        private ModSlotContainer _modSlotContainer;
        private ModSlotBehaviour _modSlotBehaviour;

        public void SetNewOrigin(ModSlotContainer targetSlotContainer, ModSlotBehaviour targetOrigin)
        {
            _modSlotContainer = targetSlotContainer;
            _modSlotBehaviour = targetOrigin;
            targetOrigin.ContainedModDragBehaviour = this;

            Transform bufferTransform = targetOrigin.transform;
            transform.position = bufferTransform.position;
            transform.SetParent(bufferTransform);
        }

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _rectTransform = GetComponent<RectTransform>();
            
            _modSlotContainer = null;
            _modSlotBehaviour = null;
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
            
            _canvasGroup.alpha = 0.5f;
            _canvasGroup.blocksRaycasts = false;
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
            dragControllerFocus.Restore();
        }
    }
}
