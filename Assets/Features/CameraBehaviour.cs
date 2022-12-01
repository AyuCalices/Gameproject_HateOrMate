using System;
using Features.GlobalReferences;
using UnityEngine;

namespace Features
{
    [RequireComponent(typeof(Camera))]
    public class CameraBehaviour : MonoBehaviour
    {
        [SerializeField] private CameraFocus_SO cameraFocus;

        private Camera _camera;
        private void Awake()
        {
            _camera = GetComponent<Camera>();
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
