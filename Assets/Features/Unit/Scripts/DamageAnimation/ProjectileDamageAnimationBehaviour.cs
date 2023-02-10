using System;
using System.Collections;
using DataStructures.Event;
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

        public override void InstantiateDamageAnimation(NetworkedBattleBehaviour casterUnit, NetworkedBattleBehaviour targetUnit, Action onHitAction)
        {
            object[] data = {targetUnit.PhotonView.ViewID};
            ProjectileDamageAnimationBehaviour instantiatedDamageAnimatorBehaviour = PhotonNetwork
                .Instantiate(gameObject.name, casterUnit.transform.position, Quaternion.identity, 0, data)
                .GetComponent<ProjectileDamageAnimationBehaviour>();
            instantiatedDamageAnimatorBehaviour.StartCoroutine(instantiatedDamageAnimatorBehaviour.CastAttack(casterUnit, targetUnit, onHitAction));

            string identifier = GetIdentifier(casterUnit.PhotonView);
            instantiatedDamageAnimatorBehaviour._identifier = identifier;
            AddToLookup(identifier, instantiatedDamageAnimatorBehaviour);
        }

        private IEnumerator CastAttack(NetworkedBattleBehaviour casterUnit, NetworkedBattleBehaviour targetUnit, Action onHitAction)
        {
            float time = Vector3.Distance(casterUnit.transform.position, targetUnit.transform.position) / speed;
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
