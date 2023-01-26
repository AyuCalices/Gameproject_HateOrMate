using ThirdParty.LeanTween.Framework;
using UnityEngine;

namespace Features.UI.Scripts
{
    public class VerticalPingPongMoveBehaviour : MonoBehaviour
    {
        [SerializeField] private Transform targetTransform;
        [SerializeField] private LeanTweenType leanTweenType;
        [SerializeField] private float time;

        private Vector3 _originPosition;

        private void Awake()
        {
            _originPosition = transform.position;
        }

        private void Start()
        {
            LeanTween.move(gameObject, targetTransform.position, time).setEase(leanTweenType).setOnComplete(() =>
            {
                LeanTween.move(gameObject, _originPosition, time).setEase(leanTweenType);
            }).setLoopPingPong();
        }
    }
}
