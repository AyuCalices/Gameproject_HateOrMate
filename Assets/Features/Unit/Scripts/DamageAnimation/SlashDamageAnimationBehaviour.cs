using System;
using System.Collections;
using Features.Unit.Scripts.Behaviours.Battle;
using Photon.Pun;
using UnityEngine;

namespace Features.Unit.Scripts.DamageAnimation
{
    public class SlashDamageAnimationBehaviour : BaseDamageAnimationBehaviour, IPunInstantiateMagicCallback
    {
        [SerializeField] private AnimationClip animationClip;
    
        private string _identifier;
    
        public override void InstantiateDamageAnimation(NetworkedBattleBehaviour casterUnit, NetworkedBattleBehaviour targetUnit,
            Action onHitAction)
        {
            SlashDamageAnimationBehaviour instantiatedDamageAnimatorBehaviour = PhotonNetwork.Instantiate(gameObject.name, casterUnit.transform.position, Quaternion.identity)
                .GetComponent<SlashDamageAnimationBehaviour>();
        
            string identifier = GetIdentifier(casterUnit.PhotonView);
            instantiatedDamageAnimatorBehaviour._identifier = identifier;
            AddToLookup(identifier, instantiatedDamageAnimatorBehaviour);
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            StartCoroutine(DestroyOnAnimationEnd());
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
