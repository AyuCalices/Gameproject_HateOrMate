using UnityEngine;

namespace Features.General.Camera.Scripts
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class CameraBehaviour : MonoBehaviour
    {
        [SerializeField] private CameraFocus_SO cameraFocus;

        private UnityEngine.Camera _camera;
        private void Awake()
        {
            _camera = GetComponent<UnityEngine.Camera>();
        }

        private void OnEnable()
        {
            cameraFocus.Set(_camera);
        }

        private void OnDisable()
        {
            cameraFocus.Restore();
        }
    }
}
