using System;
using System.Collections;
using DataStructures.Event;
using Features.Unit.Scripts.Behaviours.Battle;
using Photon.Pun;
using UnityEngine;

namespace Features.Unit.Scripts.DamageAnimation
{
    public class SlashDamageAnimationBehaviour : BaseDamageAnimationBehaviour
    {
        [SerializeField] private AnimationClip animationClip;
    
        private string _identifier;
    
        public override void InstantiateDamageAnimation(NetworkedBattleBehaviour casterUnit, NetworkedBattleBehaviour targetUnit,
            Action onHitAction)
        {
            SlashDamageAnimationBehaviour instantiatedDamageAnimatorBehaviour = PhotonNetwork
                .Instantiate(gameObject.name, targetUnit.transform.position, Quaternion.identity)
                .GetComponent<SlashDamageAnimationBehaviour>();
            instantiatedDamageAnimatorBehaviour.StartCoroutine(instantiatedDamageAnimatorBehaviour.DestroyOnAnimationEnd());
        
            onHitAction.Invoke();
            string identifier = GetIdentifier(casterUnit.PhotonView);
            instantiatedDamageAnimatorBehaviour._identifier = identifier;
            AddToLookup(identifier, instantiatedDamageAnimatorBehaviour);
        }

        private IEnumerator DestroyOnAnimationEnd()
        {
            yield return new WaitForSeconds(animationClip.length);

            Destroy();
        }
    
        private void Destroy()
        {
            RemoveFromLookup(_identifier, this);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
