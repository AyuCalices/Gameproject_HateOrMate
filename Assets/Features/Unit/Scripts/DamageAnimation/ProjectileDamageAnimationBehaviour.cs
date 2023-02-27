using System;
using System.Collections;
using DataStructures.Event;
using Features.Battle.Scripts.Unit.ServiceLocatorSystem;
using Features.Unit.Scripts.Behaviours.Battle;
using Photon.Pun;
using ThirdParty.LeanTween.Framework;
using UnityEngine;

namespace Features.Unit.Scripts.DamageAnimation
{
    public class ProjectileDamageAnimationBehaviour : BaseDamageAnimationBehaviour, IPunInstantiateMagicCallback
    {
        [SerializeField] private float speed;

        private bool _onCompleteActionCanceled;
        private string _identifier;

        public override void InstantiateDamageAnimation(UnitServiceProvider casterUnitServiceProvider, UnitServiceProvider targetUnitServiceProvider, Action onHitAction)
        {
            object[] data = {targetUnitServiceProvider.GetService<PhotonView>().ViewID};
            ProjectileDamageAnimationBehaviour instantiatedDamageAnimatorBehaviour = PhotonNetwork
                .Instantiate(gameObject.name, casterUnitServiceProvider.transform.position, Quaternion.identity, 0, data)
                .GetComponent<ProjectileDamageAnimationBehaviour>();
            instantiatedDamageAnimatorBehaviour.StartCoroutine(instantiatedDamageAnimatorBehaviour.CastAttack(casterUnitServiceProvider, targetUnitServiceProvider, onHitAction));

            string identifier = GetIdentifier(casterUnitServiceProvider.GetService<PhotonView>());
            instantiatedDamageAnimatorBehaviour._identifier = identifier;
            AddToLookup(identifier, instantiatedDamageAnimatorBehaviour);
        }

        private IEnumerator CastAttack(UnitServiceProvider casterUnitServiceProvider, UnitServiceProvider targetUnitServiceProvider, Action onHitAction)
        {
            float time = Vector3.Distance(casterUnitServiceProvider.transform.position, targetUnitServiceProvider.transform.position) / speed;
            yield return new WaitForSeconds(time);
            onHitAction.Invoke();
            Destroy();
        }

        private void Destroy()
        {
            RemoveFromLookup(_identifier, this);
            PhotonNetwork.Destroy(gameObject);
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            object[] instantiationData = info.photonView.InstantiationData;
            PhotonView targetPhotonView = PhotonView.Find((int) instantiationData[0]);
            
            if (targetPhotonView == null) return;
            
            Transform targetTransform = targetPhotonView.transform;
            float time = Vector3.Distance(transform.position, targetTransform.position) / speed;
            LeanTween.move(gameObject, targetTransform, time).setTryMoveToTransform();
        }
    }
}
