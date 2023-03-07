using UnityEngine;

namespace Features.General.UI.Scripts.CanvasFocus
{
    [RequireComponent(typeof(Canvas))]
    public class SetCanvasFocus : MonoBehaviour
    {
        [SerializeField] private CanvasFocus_SO canvasFocus;

        private void Awake()
        {
            canvasFocus.Set(GetComponent<Canvas>());
        }
    }
}
