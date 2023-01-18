using Features.Loot.UI.CharacterSelect;
using Features.Unit.Scripts.Behaviours.Mod;
using Features.Unit.Scripts.Behaviours.Stat;
using UnityEngine;

namespace Features.Loot.Scripts.ModView
{
    public class EquipView : MonoBehaviour
    {
        public UnitModViewBehaviour unitModViewPrefab;
        public Transform instantiationParent;

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
            UnitModViewBehaviour instantiatedModView = Instantiate(unitModViewPrefab, instantiationParent);
            instantiatedModView.Initialize(networkedStatsBehaviour);
            return instantiatedModView.gameObject;
        }
        
        public void ToggleEquipView()
        {
            gameObject.SetActive(!gameObject.activeInHierarchy);
        }
    }
}
