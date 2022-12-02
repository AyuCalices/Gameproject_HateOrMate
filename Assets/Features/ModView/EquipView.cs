using Features.Unit;
using Features.Unit.Modding;
using UnityEngine;

namespace Features.ModView
{
    public class EquipView : MonoBehaviour
    {
        public GameObject equipView;
        public UnitModHud unitModViewPrefab;
        public Transform instantiationParent;

        private void Awake()
        {
            equipView.SetActive(false);
        }

        private void OnEnable()
        {
            ModUnitBehaviour.onInstantiateModSlot += InstantiateModView;
        }

        private void OnDisable()
        {
            ModUnitBehaviour.onInstantiateModSlot -= InstantiateModView;
        }

        private UnitModHud InstantiateModView()
        {
            return Instantiate(unitModViewPrefab, instantiationParent);
        }

        public void ToggleEquipView()
        {
            equipView.SetActive(!equipView.activeInHierarchy);
        }
    }
}
