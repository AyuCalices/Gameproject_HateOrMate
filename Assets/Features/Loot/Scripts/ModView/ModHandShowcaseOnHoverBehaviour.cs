using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Features.Loot.Scripts.ModView
{
    public class ModHandShowcaseOnHoverBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private GameObject blankedModPrefab;
        
        private GameObject _hoverTempGapObject;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            ModHandShowcaseOnHover();
        }
    
        private void ModHandShowcaseOnHover()
        {
            Transform parent = transform.parent;
            int siblingIndex = transform.GetSiblingIndex();
            if (parent.childCount - 1 != siblingIndex)
            {
                _hoverTempGapObject = Instantiate(blankedModPrefab, parent);
                RectTransform rectTransform = (RectTransform) _hoverTempGapObject.transform;
                Vector2 sizeDelta = rectTransform.sizeDelta;
                rectTransform.sizeDelta = new Vector2(Mathf.Abs(parent.GetComponent<HorizontalLayoutGroup>().spacing) * 2, sizeDelta.y);
                _hoverTempGapObject.transform.SetSiblingIndex(siblingIndex + 1);
            }
        }
    
        public void OnPointerExit(PointerEventData eventData)
        {
            CleanupModHandShowcaseOnHover();
        }
    
        private void CleanupModHandShowcaseOnHover()
        {
            Destroy(_hoverTempGapObject);
        }
    }
}
