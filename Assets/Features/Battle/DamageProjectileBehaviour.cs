using System;
using Photon.Pun;
using ThirdParty.LeanTween.Framework;
using UnityEngine;

namespace Features.Battle
{
    public class DamageProjectileBehaviour : MonoBehaviour, IPunInstantiateMagicCallback
    {
        [SerializeField] private float speed;

        private Action _onCompleteAction;
        private bool _onCompleteActionCanceled;
        
        
        public DamageProjectileBehaviour FireProjectile(Vector3 startPosition, Vector3 targetPosition)
        {
            return InstantiateProjectile(startPosition, targetPosition).GetComponent<DamageProjectileBehaviour>();
        }

        public void RegisterOnCompleteAction(Action onCompleteAction)
        {
            _onCompleteAction = onCompleteAction;
        }

        public void CancelProjectile()
        {
            _onCompleteActionCanceled = true;
            LeanTween.cancel(gameObject, true);
        }
        
        private GameObject InstantiateProjectile(Vector3 startPosition, Vector3 targetPosition)
        {
            object[] data = new object[] {targetPosition};
            return PhotonNetwork.Instantiate("Projectile", startPosition, Quaternion.identity, 0, data);
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            object[] instantiationData = info.photonView.InstantiationData;
            Vector3 targetPosition = (Vector3) instantiationData[0];
            
            float time = Vector3.Distance(transform.position, targetPosition) / speed;
            LeanTween.move(gameObject, targetPosition, time).setOnComplete(() =>
            {
                if (!info.photonView.IsMine) return;
                
                if (!_onCompleteActionCanceled)
                {
                    _onCompleteAction.Invoke();
                }
                
                PhotonNetwork.Destroy(gameObject);
            });
        }
    }
}
