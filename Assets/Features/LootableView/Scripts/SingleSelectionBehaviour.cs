using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Features.Loot.Scripts.LootView
{
    public class SingleSelectionBehaviour : MonoBehaviour
    {
        [SerializeField] private GameObject selector;
    
        private static SingleSelectionBehaviour _currentSelection;
    
        private void Awake()
        {
            selector.SetActive(false);
        
            Button button = GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                DisableCurrentSelection();
                EnableCurrentSelection();
            });
        }
    
        private void OnDestroy()
        {
            DisableCurrentSelection();
        }
    
        public void DisableCurrentSelection()
        {
            if (_currentSelection == null) return;
            
            _currentSelection.selector.SetActive(false);
            _currentSelection = null;
        }
        
        public void EnableCurrentSelection()
        {
            if (_currentSelection != null) return;
            
            _currentSelection = this;
            _currentSelection.selector.SetActive(true);
        }
    }
}
