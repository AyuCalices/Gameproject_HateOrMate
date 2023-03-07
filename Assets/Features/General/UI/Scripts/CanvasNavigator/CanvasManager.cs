using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Features.UI.Scripts.CanvasNavigator
{
    public class CanvasManager : MonoBehaviour
    {
        private List<CanvasController> _canvasControllerList;
        private CanvasController _lastActiveCanvas;

        private void Start()
        {
            _canvasControllerList = GetComponentsInChildren<CanvasController>().ToList();
            _canvasControllerList.ForEach(x => x.gameObject.SetActive(false));
        
            MenuType_SO startingMenu = _canvasControllerList.Find(controller => controller.isStartMenu).canvasType;
            SwitchCanvas(startingMenu);
        }

        public void SwitchCanvas(MenuType_SO type)
        {
            if (_lastActiveCanvas != null)
            {
                _lastActiveCanvas.gameObject.SetActive(false);
            }
            
            CanvasController desiredCanvas = _canvasControllerList.Find(x => x.canvasType == type);
            if (desiredCanvas != null)
            {
                desiredCanvas.gameObject.SetActive(true);
                _lastActiveCanvas = desiredCanvas;
            }
            else
            {
                Debug.LogWarning("Desired canvas was not found");
            }
        }

        public void CloseCanvas()
        {
            if (_lastActiveCanvas != null)
            {
                _lastActiveCanvas.gameObject.SetActive(false);
                _lastActiveCanvas = null;
            }
            else
            {
                Debug.LogWarning("No last active canvas");
            }
        
        }
    
    }
}
