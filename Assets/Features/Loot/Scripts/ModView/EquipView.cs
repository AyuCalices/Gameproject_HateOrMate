using System.Collections.Generic;
using Features.Loot.Scripts.GeneratedLoot;
using Features.Unit.Scripts.Behaviours.Mod;
using UnityEngine;

namespace Features.Loot.Scripts.ModView
{
    public class EquipView : MonoBehaviour
    {
        public GameObject unitModViewPrefab;
        public Transform instantiationParent;

        private void Awake()
        {
            gameObject.SetActive(false);
            ModUnitBehaviour.onInstantiateModSlot += InstantiateModView;
        }

        private void OnDestroy()
        {
            ModUnitBehaviour.onInstantiateModSlot -= InstantiateModView;
        }

        private GameObject InstantiateModView()
        {
            return Instantiate(unitModViewPrefab, instantiationParent);
        }
        
        public void ToggleEquipView()
        {
            gameObject.SetActive(!gameObject.activeInHierarchy);
        }
    }
}
