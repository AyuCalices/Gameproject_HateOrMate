using UnityEngine;
using UnityEngine.UI;

namespace Features.Loot.Scripts.LootView
{
    public class SingleSelectionBehaviour : MonoBehaviour
    {
        [SerializeField] private GameObject selection;
    
        private static SingleSelectionBehaviour _currentSelection;
    
        private void Awake()
        {
            selection.SetActive(false);
        
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
            
            _currentSelection.selection.SetActive(false);
            _currentSelection = null;
        }
        
        public void EnableCurrentSelection()
        {
            if (_currentSelection != null) return;
            
            _currentSelection = this;
            _currentSelection.selection.SetActive(true);
        }
    }
}
