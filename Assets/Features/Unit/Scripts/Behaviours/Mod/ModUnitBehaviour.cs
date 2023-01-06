using System;
using Features.Loot.Scripts.ModView;
using Features.Unit.Scripts.Behaviours.Stat;
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
    
        // Start is called before the first frame update
        private void Awake()
        {
            NetworkedStatsBehaviour networkedStatsBehaviour = GetComponent<NetworkedStatsBehaviour>();
            _unitModGameObject = onInstantiateModSlot.Invoke(networkedStatsBehaviour);
            ModSlotBehaviour[] modDropBehaviours = _unitModGameObject.GetComponentsInChildren<ModSlotBehaviour>();
            UnitMods = new UnitMods(networkedStatsBehaviour, modDropBehaviours);
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
