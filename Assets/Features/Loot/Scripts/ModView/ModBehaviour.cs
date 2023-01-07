using Features.Loot.Scripts.GeneratedLoot;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Features.Loot.Scripts.ModView
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ModBehaviour : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [SerializeField] private CanvasFocus_SO canvasFocus;
        [SerializeField] private Image image;
        [SerializeField] private Transform imageParent;
        [SerializeField] private TMP_Text description;
        [SerializeField] private TMP_Text level;
        
        public BaseMod ContainedMod { get; private set; }
        public IModContainer CurrentModContainer { get; private set; }
        public bool IsSuccessfulDrop { get; set; }
        public bool IsInHand { get; set; }
        public ExpandBehaviour ExpandBehaviour { get; private set; }
        
        private CanvasGroup _canvasGroup;
        private RectTransform _rectTransform;
        private Vector3 _dragBeginPosition;
        private Transform _originTransform;
        private Transform _dragTransform;
        
        public void Initialize(BaseMod baseMod, Transform dragTransform, IModContainer currentModContainer)
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _rectTransform = GetComponent<RectTransform>();
            ExpandBehaviour = GetComponent<ExpandBehaviour>();
            ContainedMod = baseMod;

            CurrentModContainer = currentModContainer;
            _originTransform = transform.parent;
            _dragTransform = dragTransform;
            
            Instantiate(baseMod.SpritePrefab, imageParent);
            description.text = baseMod.Description;
            level.text = baseMod.Level.ToString();
        }
        
        public void SetNewOrigin(IModContainer targetOrigin)
        {
            CurrentModContainer = targetOrigin;
            _originTransform = targetOrigin.Transform;
            transform.position = _originTransform.position;
            transform.SetParent(_originTransform);
        }

        public void UpdateColor(Color newColor)
        {
            image.color = newColor;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            transform.SetParent(_dragTransform);
            _canvasGroup.alpha = 0.5f;
            _canvasGroup.blocksRaycasts = false;

            if (IsInHand)
            {
                ExpandBehaviour.IsActive = true;
            }
            
            IsSuccessfulDrop = false;
            _dragBeginPosition = _dragTransform.position;
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

            if (!IsSuccessfulDrop)
            {
                if (IsInHand)
                {
                    ExpandBehaviour.SetExpanded(true);
                    ExpandBehaviour.IsActive = false;
                }
                transform.position = _dragBeginPosition;
            }
            
            transform.position = _originTransform.position;
            transform.SetParent(_originTransform);
        }
    }
}
