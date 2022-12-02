using System;
using System.Collections.Generic;
using Features.ModView;
using UnityEngine;

namespace Features.Unit.Modding
{
    [RequireComponent(typeof(LocalUnitBehaviour))]
    public class ModUnitBehaviour : MonoBehaviour
    {
        [SerializeField] private int modCount;
    
        public UnitMods UnitMods { get; private set; }
    
        public static Func<UnitModHud> onInstantiateModSlot;
    
        // Start is called before the first frame update
        private void Awake()
        {
            UnitModHud unitModView = onInstantiateModSlot.Invoke();
            List<ModSlotBehaviour> modDropBehaviours = unitModView.GetAllChildren();
            UnitMods = new UnitMods(modCount, GetComponent<LocalUnitBehaviour>(), modDropBehaviours);
        }
    }
}
