using System.Collections.Generic;
using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Mod;
using UnityEngine;

namespace Features.Loot.Scripts.ModView
{
    public class EquipView : MonoBehaviour
    {
        [SerializeField] private UnitViewBehaviour unitViewPrefab;
        [SerializeField] private Transform instantiationParent;

        private void Awake()
        {
            ModUnitBehaviour.onInstantiateModSlot += InstantiateModView;
        }

        private void OnDestroy()
        {
            ModUnitBehaviour.onInstantiateModSlot -= InstantiateModView;
        }

        private GameObject InstantiateModView(NetworkedStatsBehaviour networkedStatsBehaviour)
        {
            UnitViewBehaviour instantiatedView = Instantiate(unitViewPrefab, instantiationParent);
            instantiatedView.Initialize(networkedStatsBehaviour);
            return instantiatedView.gameObject;
        }
    }
}
