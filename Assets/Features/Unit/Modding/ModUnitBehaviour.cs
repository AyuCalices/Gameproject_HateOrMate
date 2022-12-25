using System;
using System.Collections.Generic;
using Features.GlobalReferences.Scripts;
using Features.ModView;
using UnityEngine;

namespace Features.Unit.Modding
{
    [RequireComponent(typeof(NetworkedStatsBehaviour))]
    public class ModUnitBehaviour : MonoBehaviour
    {
        [SerializeField] private ModUnitRuntimeSet_SO modUnitRuntimeSet;
        
        [Header("Balancing")]
        [SerializeField] private int modCount;
    
        public UnitMods UnitMods { get; private set; }
    
        public static Func<UnitModHud> onInstantiateModSlot;

        private UnitModHud _unitModView;
    
        // Start is called before the first frame update
        private void Awake()
        {
            _unitModView = onInstantiateModSlot.Invoke();
            List<ModSlotBehaviour> modDropBehaviours = _unitModView.GetAllChildren();
            UnitMods = new UnitMods(modCount, GetComponent<NetworkedStatsBehaviour>(), modDropBehaviours);
            modUnitRuntimeSet.Add(this);
        }

        private void OnDestroy()
        {
            modUnitRuntimeSet.Remove(this);
            Destroy(_unitModView.gameObject);
        }
    }
}
