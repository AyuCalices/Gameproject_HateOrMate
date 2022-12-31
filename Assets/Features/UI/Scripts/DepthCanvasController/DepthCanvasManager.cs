using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace Features.UI.Scripts.DepthCanvasController
{
    public class DepthCanvasManager : MonoBehaviour
    {
        [SerializeField] private MenuType_SO startingMenu;
        
        private List<DepthCanvasController> canvasControllerList;
    
        [ReadOnly] private readonly List<DepthCanvasController> canvasCommandList = new List<DepthCanvasController>();
        private DepthCanvasController CurrentDepthCanvas => canvasCommandList[canvasCommandList.Count - 1];

        private void Awake()
        {
            canvasControllerList = GetComponentsInChildren<DepthCanvasController>().ToList();
            canvasControllerList.ForEach(x => x.gameObject.SetActive(false));

            AddCanvas(startingMenu);
        }

        public void HideCanvas()
        {
            CurrentDepthCanvas.gameObject.SetActive(false);
        }
    
        public void ShowCanvas()
        {
            CurrentDepthCanvas.gameObject.SetActive(true);
        }

        public void AddCanvas(MenuType_SO type)
        {
            if (canvasCommandList.Count != 0)
            {
                CurrentDepthCanvas.gameObject.SetActive(false);
            }
        
            DepthCanvasController desiredDepthCanvas = canvasControllerList.Find(x => x.canvasType == type);
            if (desiredDepthCanvas != null)
            {
                desiredDepthCanvas.gameObject.SetActive(true);
                canvasCommandList.Add(desiredDepthCanvas);
            }
            else
            {
                Debug.LogWarning("Desired canvas was not found");
            }
        }

        public void RemoveCanvas_SetActive(bool setActive)
        {
            if (canvasCommandList.Count > 0)
            {
                HideCanvas();
                canvasCommandList.Remove(CurrentDepthCanvas);

                if (setActive)
                {
                    ShowCanvas();
                }
            }
            else
            {
                Debug.LogError($"You cant remove the menu added first to the {name}");
            }
        }

        public void RemoveCanvasTo(MenuType_SO type)
        {
            if (canvasCommandList.Count > 0)
            {
                DepthCanvasController desiredDepthCanvas = canvasCommandList.Find(x => x.canvasType == type);
            
                if (desiredDepthCanvas != null)
                {
                    HideCanvas();
                
                    int canvasIndex = canvasCommandList.FindIndex(x => x.canvasType == type);
                    canvasCommandList.RemoveRange(canvasIndex + 1, canvasCommandList.Count - 1 - canvasIndex);
                
                    desiredDepthCanvas.gameObject.SetActive(true);
                }
                else
                {
                    Debug.LogError("The requested menu wasn't opened before!");
                }
            }
            else
            {
                Debug.LogError($"You cant remove the menu added first to the {name}");
            }
        }
    }
}
