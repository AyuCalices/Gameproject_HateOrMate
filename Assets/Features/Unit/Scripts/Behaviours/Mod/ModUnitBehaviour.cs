using System;
using Features.Loot.Scripts.ModView;
using UnityEngine;

namespace Features.Unit.Scripts.Behaviours.Mod
{
    [RequireComponent(typeof(NetworkedStatsBehaviour))]
    public class ModUnitBehaviour : MonoBehaviour
    {
        [SerializeField] private ModUnitRuntimeSet_SO modUnitRuntimeSet;
    
        public UnitMods UnitMods { get; private set; }
    
        public static Func<NetworkedStatsBehaviour, GameObject> onInstantiateModSlot;

        private GameObject _unitModGameObject;
        private NetworkedStatsBehaviour _networkedStatsBehaviour;
    
        public void InstantiateModView()
        {
            _networkedStatsBehaviour = GetComponent<NetworkedStatsBehaviour>();
            _unitModGameObject = onInstantiateModSlot.Invoke(_networkedStatsBehaviour);
            ModSlotBehaviour[] modDropBehaviours = _unitModGameObject.GetComponentsInChildren<ModSlotBehaviour>();
            UnitMods = new UnitMods(_networkedStatsBehaviour, modDropBehaviours);
            modUnitRuntimeSet.Add(this);
        }

        public void OnDestroy()
        {
            modUnitRuntimeSet.Remove(this);
            UnitMods.OnDestroy();
            Destroy(_unitModGameObject);
        }
    }
}
