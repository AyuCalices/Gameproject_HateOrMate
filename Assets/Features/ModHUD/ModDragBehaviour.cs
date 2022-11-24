using Features.Mod;
using Features.Unit;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Features.ModHUD
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ModDragBehaviour : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [SerializeField] private DragControllerFocus_SO dragControllerFocus;
        [SerializeField] private BaseModGenerator_SO baseModGenerator;
        [SerializeField] private Canvas canvas;
        
        private CanvasGroup _canvasGroup;
        private RectTransform _rectTransform;

        private BaseMod _baseMod;
        private ModSlotContainer _modSlotContainer;
        private ModSlotBehaviour _modSlotBehaviour;

        public void SetNewOrigin(ModSlotContainer originSlotContainer, ModSlotBehaviour newOrigin)
        {
            _modSlotContainer = originSlotContainer;
            _modSlotBehaviour = newOrigin;
            newOrigin.ContainedModDragBehaviour = this;

            Transform bufferTransform = newOrigin.transform;
            transform.position = bufferTransform.position;
            transform.SetParent(bufferTransform);
        }

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _rectTransform = GetComponent<RectTransform>();

            _baseMod = baseModGenerator.Generate();
            
            _modSlotContainer = null;
            _modSlotBehaviour = null;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            dragControllerFocus.Set(new DragController(this, _baseMod, _modSlotContainer, _modSlotBehaviour));
            
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
            dragControllerFocus.Restore();
        }
    }
}
