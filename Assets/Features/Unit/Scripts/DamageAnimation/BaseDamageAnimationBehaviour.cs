using System;
using System.Collections.Generic;
using Features.Unit.Scripts.Behaviours.Battle;
using Photon.Pun;
using UnityEngine;

namespace Features.Unit.Scripts.DamageAnimation
{
    public abstract class BaseDamageAnimationBehaviour : MonoBehaviour
    {
        [field: SerializeField] private bool destroyOnStageClear;

        private static readonly Dictionary<string, List<BaseDamageAnimationBehaviour>> _damageAnimationLookup = new Dictionary<string, List<BaseDamageAnimationBehaviour>>();
        
        public abstract void InstantiateDamageAnimation(NetworkedBattleBehaviour casterUnit, NetworkedBattleBehaviour targetUnit, Action onHitAction);

        public string GetIdentifier(PhotonView ownerPhotonView) => ownerPhotonView.ViewID + gameObject.name;

        public static void DestroyAllByPrefabReference(BaseDamageAnimationBehaviour baseDamageAnimationPrefab, PhotonView ownerPhotonView)
        {
            string identifier = baseDamageAnimationPrefab.GetIdentifier(ownerPhotonView);
            
            if (!_damageAnimationLookup.TryGetValue(identifier, out List<BaseDamageAnimationBehaviour> instantiatedDamageAnimatorBehaviours)) return;

            for (int index = instantiatedDamageAnimatorBehaviours.Count - 1; index >= 0; index--)
            {
                BaseDamageAnimationBehaviour instantiatedDamageAnimatorBehaviour = instantiatedDamageAnimatorBehaviours[index];
                
                instantiatedDamageAnimatorBehaviour.RemoveFromLookup(identifier, instantiatedDamageAnimatorBehaviour);
                if (instantiatedDamageAnimatorBehaviour.destroyOnStageClear)
                {
                    PhotonNetwork.Destroy(instantiatedDamageAnimatorBehaviour.gameObject);
                }
            }
        }
        
        protected void AddToLookup(string identifier, BaseDamageAnimationBehaviour instantiatedDamageAnimatorBehaviour)
        {
            if (_damageAnimationLookup.TryGetValue(identifier, out List<BaseDamageAnimationBehaviour> instantiatedDamageAnimatorBehaviours))
            {
                instantiatedDamageAnimatorBehaviours.Add(instantiatedDamageAnimatorBehaviour);
            }
            else
            {
                _damageAnimationLookup.Add(identifier, new List<BaseDamageAnimationBehaviour>(){instantiatedDamageAnimatorBehaviour});
            }
        }

        protected void RemoveFromLookup(string identifier, BaseDamageAnimationBehaviour instantiatedDamageAnimatorBehaviour)
        {
            if (_damageAnimationLookup.TryGetValue(identifier, out List<BaseDamageAnimationBehaviour> instantiatedDamageAnimatorBehaviours))
            {
                instantiatedDamageAnimatorBehaviours.Remove(instantiatedDamageAnimatorBehaviour);
            }
        }
    }
}
