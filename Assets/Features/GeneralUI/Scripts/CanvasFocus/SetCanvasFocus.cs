using UnityEngine;

namespace Features.Loot.Scripts.ModView
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
