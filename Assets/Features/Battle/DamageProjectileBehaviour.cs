using Photon.Pun;
using ThirdParty.LeanTween.Framework;
using UnityEngine;

namespace Features.Battle
{
    public class DamageProjectileBehaviour : MonoBehaviour, IPunInstantiateMagicCallback
    {
        [SerializeField] private float speed;

        public void PhotonInstantiate(Vector3 startPosition, Vector3 targetPosition)
        {
            object[] data = new object[] {targetPosition};
            PhotonNetwork.Instantiate("Projectile", startPosition, Quaternion.identity, 0, data);
        }

        public float GetTime(Vector3 startPosition, Vector3 targetPosition)
        {
            return Vector3.Distance(startPosition, targetPosition) / speed;
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            object[] instantiationData = info.photonView.InstantiationData;
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = (Vector3) instantiationData[0];
            float time = GetTime(startPosition, targetPosition);
            LeanTween.move(gameObject, targetPosition, time).setOnComplete(() =>
            {
                if (!info.photonView.IsMine) return;
                PhotonNetwork.Destroy(gameObject);
            });
        }
    }
}
