using System;
using System.Collections.Generic;
using Features.ModView;
using Features.Unit.Stat;
using UnityEngine;

namespace Features.Unit
{
    //TODO: implement base stat values
    public class LocalUnitBehaviour : NetworkedUnitBehaviour
    {
        public static Func<UnitModHud> onInstantiateModSlot;
        
        [SerializeField] private LocalUnitRuntimeSet_SO localPlayerLocalUnits;
        [SerializeField] private int modCount;

        public LocalUnitRuntimeSet_SO LocalPlayerLocalUnits => localPlayerLocalUnits;
        
        public UnitMods UnitMods { get; private set; }

        protected override void InternalAwake()
        {
            UnitModHud unitModView = onInstantiateModSlot.Invoke();
            List<ModSlotBehaviour> modDropBehaviours = unitModView.GetAllChildren();
            UnitMods = new UnitMods(modCount, this, modDropBehaviours);
        }

        protected override void InternalStart()
        {
            //implement stuff
        }

        protected void Update()
        {
            //Debug.Log(NetworkedStatServiceLocator.GetTotalValue(StatType.Damage));
        }

        protected override void AddToRuntimeSet()
        {
            localPlayerLocalUnits.Add(this);
        }
        
        protected override void RemoveFromRuntimeSet()
        {
            localPlayerLocalUnits.Remove(this);
        }
    }
}
