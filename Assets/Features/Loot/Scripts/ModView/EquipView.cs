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
        public Transform modInstantiationParent;

        private List<ModDragBehaviour> _modDragBehaviourPrefab = new List<ModDragBehaviour>();

        private void Awake()
        {
            gameObject.SetActive(false);
            ModUnitBehaviour.onInstantiateModSlot += InstantiateModView;
            BaseMod.onModInstantiated += InstantiateModToHand;
        }

        private void OnDestroy()
        {
            ModUnitBehaviour.onInstantiateModSlot -= InstantiateModView;
            BaseMod.onModInstantiated -= InstantiateModToHand;
        }

        private GameObject InstantiateModView()
        {
            return Instantiate(unitModViewPrefab, instantiationParent);
        }

        private void InstantiateModToHand(ModDragBehaviour modDragBehaviourPrefab, BaseMod baseMod)
        {
            ModDragBehaviour modDragBehaviour = Instantiate(modDragBehaviourPrefab, modInstantiationParent);
            modDragBehaviour.Initialize(baseMod, transform);
        }

        public void ToggleEquipView()
        {
            gameObject.SetActive(!gameObject.activeInHierarchy);
        }
    }
}
