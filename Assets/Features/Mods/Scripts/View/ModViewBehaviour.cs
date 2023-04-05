using Features.General.UI.Scripts;
using Features.General.UI.Scripts.CanvasFocus;
using Features.Mods.Scripts.ModTypes;
using Features.Mods.Scripts.View.ModContainer;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Features.Mods.Scripts.View
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ModViewBehaviour : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler
    {
        [SerializeField] private CanvasFocus_SO canvasFocus;
        [SerializeField] private Image image;
        [SerializeField] private Transform imageParent;
        [SerializeField] private TMP_Text description;
        [SerializeField] private TMP_Text level;
        
        public BaseMod ContainedMod { get; private set; }
        public IModContainer CurrentModContainer { get; private set; }
        public ExpandBehaviour ExpandBehaviour { get; private set; }

        private CanvasGroup _canvasGroup;
        private RectTransform _rectTransform;
        private Transform _dragTransform;
        private ScrollRect _scrollRect;
        private static ModViewBehaviour _modViewSelection;

        private bool _isSelected;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _rectTransform = GetComponent<RectTransform>();
            _scrollRect = GetComponentInParent<ScrollRect>();
            ExpandBehaviour = GetComponent<ExpandBehaviour>();
        }

        private void Start()
        {
            SetExpanded();
        }

        public void Initialize(BaseMod baseMod, Transform dragTransform, IModContainer currentModContainer)
        {
            ContainedMod = baseMod;

            SetNewOrigin(currentModContainer);
            _dragTransform = dragTransform;
            
            Instantiate(baseMod.SpritePrefab, imageParent);
            description.text = baseMod.Description;
            level.text = baseMod.Level.ToString();
        }
        
        public void InitializeNewOrigin(IModContainer targetOrigin)
        {
            SetNewOrigin(targetOrigin);
            SetExpanded();
        }

        private void SetNewOrigin(IModContainer targetOrigin)
        {
            CurrentModContainer = targetOrigin;
            transform.SetParent(CurrentModContainer.Transform);
            transform.position = CurrentModContainer.Transform.position;
        }

        private void SetExpanded() 
        {
            bool isInHand = CurrentModContainer is HandModContainerBehaviour;
            ExpandBehaviour.SetExpanded(isInHand);
            ExpandBehaviour.enabled = !isInHand;
        }

        public void UpdateColor(Color newColor)
        {
            image.color = newColor;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            int siblingIndex = transform.GetSiblingIndex();
            if (CurrentModContainer.Transform.childCount - 1 == siblingIndex)
            {
                transform.SetParent(_dragTransform);
                _canvasGroup.alpha = 0.5f;
                _canvasGroup.blocksRaycasts = false;

                ExpandBehaviour.SetExpanded(false);
            }
            else
            {
                eventData.pointerDrag = _scrollRect.gameObject;
                EventSystem.current.SetSelectedGameObject(_scrollRect.gameObject);
                _scrollRect.OnInitializePotentialDrag(eventData);    
                _scrollRect.OnBeginDrag(eventData);
            }
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

            if (transform.parent == CurrentModContainer.Transform) return;
            
            transform.SetParent(CurrentModContainer.Transform);
            transform.position = CurrentModContainer.Transform.position;
            
            SetExpanded();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (CurrentModContainer is HandModContainerBehaviour)
            {
                transform.SetAsLastSibling();
                _scrollRect.horizontalNormalizedPosition = 1f;
            }
            else if (CurrentModContainer is UnitModContainerBehaviour)
            {
                if (_modViewSelection != null && _modViewSelection.CurrentModContainer is UnitModContainerBehaviour)
                {
                    if (_modViewSelection != this)
                    {
                        _modViewSelection.ExpandBehaviour.SetExpanded(false);
                    }
                }

                _modViewSelection = this;
            }
        }
    }
}
