using System.Collections.Generic;
using Features.Unit.Scripts.Behaviours.Mod;
using Features.Unit.Scripts.Behaviours.Stat;
using UnityEngine;

namespace Features.Loot.Scripts.ModView
{
    public class EquipView : MonoBehaviour
    {
        [SerializeField] private List<TeamViewBehaviour> teamView;
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
            foreach (TeamViewBehaviour teamViewBehaviour in teamView)
            {
                teamViewBehaviour.Initialize();
            }
            
            UnitViewBehaviour instantiatedView = Instantiate(unitViewPrefab, instantiationParent);
            instantiatedView.Initialize(networkedStatsBehaviour);
            return instantiatedView.gameObject;
        }
    }
}
