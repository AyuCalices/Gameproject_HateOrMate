using System;
using ThirdParty.LeanTween.Framework;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Features.Loot.Scripts.ModView
{
    public enum AxisType { Horizontal, Vertical}
    public class ExpandBehaviour : MonoBehaviour, IPointerClickHandler
    {
        public AxisType axisType;
        public bool expandOnAwake;
        public float expanded = 1430;
        public float normal = 390;
        public float time = 0.2f;
        public LeanTweenType easeType;

        public bool IsExpanded { get; private set; }
        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            SetExpandedWithoutTween(expandOnAwake);
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            SetExpanded(!IsExpanded);
        }

        public void SetExpanded(bool expand)
        {
            if (expand)
            {
                if (axisType == AxisType.Horizontal)
                {
                    LeanTween.size(_rectTransform, new Vector2(expanded, _rectTransform.sizeDelta.y), time)
                        .setEase(easeType);
                }
                else
                {
                    LeanTween.size(_rectTransform, new Vector2(_rectTransform.sizeDelta.x, expanded), time)
                        .setEase(easeType);
                }
                
                IsExpanded = true;
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
                
                IsExpanded = false;
            }
        }

        private void SetExpandedWithoutTween(bool expand)
        {
            if (expand)
            {
                if (axisType == AxisType.Horizontal)
                {
                    _rectTransform.sizeDelta = new Vector2(expanded, _rectTransform.sizeDelta.y);
                }
                else
                {
                    _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, expanded);
                }
                
                IsExpanded = true;
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
                
                IsExpanded = false;
            }
        }
    }
}
