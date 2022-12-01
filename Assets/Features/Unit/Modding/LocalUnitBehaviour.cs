using System;
using System.Collections.Generic;
using Features.GlobalReferences;
using Features.ModView;
using Features.Unit.Modding.Stat;
using UnityEngine;

namespace Features.Unit.Modding
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
            NetworkedStatServiceLocator.TryAddLocalValue(StatType.Damage, StatValueType.Stat, 10);
            NetworkedStatServiceLocator.TryAddLocalValue(StatType.Damage, StatValueType.ScalingStat, 1);
            
            NetworkedStatServiceLocator.TryAddLocalValue(StatType.Health, StatValueType.Stat, 50);
            NetworkedStatServiceLocator.TryAddLocalValue(StatType.Health, StatValueType.ScalingStat, 1);
            
            NetworkedStatServiceLocator.TryAddLocalValue(StatType.Speed, StatValueType.Stat, 3);
            NetworkedStatServiceLocator.TryAddLocalValue(StatType.Speed, StatValueType.ScalingStat, 1);
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
