using Features.GlobalReferences;
using Features.GlobalReferences.Scripts;
using UnityEngine;

namespace Features.ModView
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
