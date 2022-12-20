using System;
using System.Collections.Generic;
using Features.ModView;
using UnityEngine;

namespace Features.Unit.Modding
{
    [RequireComponent(typeof(NetworkedStatsBehaviour))]
    public class ModUnitBehaviour : MonoBehaviour
    {
        [Header("Balancing")]
        [SerializeField] private int modCount;
    
        public UnitMods UnitMods { get; private set; }
    
        public static Func<UnitModHud> onInstantiateModSlot;
    
        // Start is called before the first frame update
        private void Awake()
        {
            UnitModHud unitModView = onInstantiateModSlot.Invoke();
            List<ModSlotBehaviour> modDropBehaviours = unitModView.GetAllChildren();
            //TODO: getComponent
            UnitMods = new UnitMods(modCount, GetComponent<NetworkedStatsBehaviour>(), modDropBehaviours);
        }
    }
}
