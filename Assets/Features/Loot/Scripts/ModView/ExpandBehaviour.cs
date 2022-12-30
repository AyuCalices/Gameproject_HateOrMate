using System;
using ThirdParty.LeanTween.Framework;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Features.Loot.Scripts.ModView
{
    public enum ExpandType { OnClick, OnHover }
    public enum AxisType { Horizontal, Vertical}
    public class ExpandBehaviour : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public AxisType axisType;
        public ExpandType expandType;
        public bool enabledAtAwake;
        public bool expandOnAwake;
        public float expanded = 1430;
        public float normal = 390;
        public float time = 0.2f;
        public LeanTweenType easeType;

        private bool _isExpanded;
        private RectTransform _rectTransform;
        
        public bool IsActive { get; set; }

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            IsActive = enabledAtAwake;
            SetExpandedWithoutTween(expandOnAwake);
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (expandType != ExpandType.OnClick || !IsActive) return;
            
            SetExpanded(_isExpanded);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (expandType != ExpandType.OnHover || !IsActive) return;
            
            SetExpanded(_isExpanded);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (expandType != ExpandType.OnHover || !IsActive) return;
            
            SetExpanded(_isExpanded);
        }

        public void SetExpanded(bool expanded)
        {
            if (expanded)
            {
                if (axisType == AxisType.Horizontal)
                {
                    LeanTween.size(_rectTransform, new Vector2(this.expanded, _rectTransform.sizeDelta.y), time)
                        .setEase(easeType);
                }
                else
                {
                    LeanTween.size(_rectTransform, new Vector2(_rectTransform.sizeDelta.x, this.expanded), time)
                        .setEase(easeType);
                }
                
                _isExpanded = false;
            }
            else
            {
                if (axisType == AxisType.Horizontal)
                {
                    LeanTween.size(_rectTransform, new Vector2(normal, _rectTransform.sizeDelta.y), time)
                        .setEase(easeType);
                }
                else
                {
                    LeanTween.size(_rectTransform, new Vector2(_rectTransform.sizeDelta.x, normal), time)
                        .setEase(easeType);
                }
                
                _isExpanded = true;
            }
        }

        private void SetExpandedWithoutTween(bool expanded)
        {
            if (expanded)
            {
                if (axisType == AxisType.Horizontal)
                {
                    _rectTransform.sizeDelta = new Vector2(this.expanded, _rectTransform.sizeDelta.y);
                }
                else
                {
                    _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, this.expanded);
                }
                
                _isExpanded = false;
            }
            else
            {
                if (axisType == AxisType.Horizontal)
                {
                    _rectTransform.sizeDelta =  new Vector2(normal, _rectTransform.sizeDelta.y);
                }
                else
                {
                    _rectTransform.sizeDelta =  new Vector2(_rectTransform.sizeDelta.x, normal);
                }
                
                _isExpanded = true;
            }
        }
    }
}
