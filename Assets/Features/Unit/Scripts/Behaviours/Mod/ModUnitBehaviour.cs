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
    
        public static Func<GameObject> onInstantiateModSlot;

        private GameObject _unitModGameObject;
    
        // Start is called before the first frame update
        private void Awake()
        {
            _unitModGameObject = onInstantiateModSlot.Invoke();
            ModSlotBehaviour[] modDropBehaviours = _unitModGameObject.GetComponentsInChildren<ModSlotBehaviour>();
            UnitMods = new UnitMods(GetComponent<NetworkedStatsBehaviour>(), modDropBehaviours);
            modUnitRuntimeSet.Add(this);
        }

        public void OnDestroy()
        {
            modUnitRuntimeSet.Remove(this);
            Destroy(_unitModGameObject);
            UnitMods.OnDestroy();
            
            //TODO: move all mods to hand
        }
    }
}
