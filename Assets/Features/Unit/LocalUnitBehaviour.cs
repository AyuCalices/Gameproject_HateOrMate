using System.Collections.Generic;
using Features.Unit.Stat;
using UnityEngine;

namespace Features.Unit
{
    public class LocalUnitBehaviour : NetworkedUnitBehaviour
    {
        [SerializeField] private Canvas unitModHUD;
        [SerializeField] private LocalUnitRuntimeSet_SO localPlayerLocalUnits;
        [SerializeField] private int modCount;

        public LocalUnitRuntimeSet_SO LocalPlayerLocalUnits => localPlayerLocalUnits;
        
        public UnitMods UnitMods { get; private set; }

        protected override void InternalAwake()
        {
            List<ModSlotBehaviour> modDropBehaviours = Instantiate(unitModHUD).GetComponentInChildren<UnitModHud>().GetAllChildren();
            UnitMods = new UnitMods(modCount, this, modDropBehaviours);
        }

        protected override void InternalStart()
        {
            //implement stuff
        }

        protected void Update()
        {
            Debug.Log(NetworkedStatServiceLocator.GetTotalValue(StatType.Damage));
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
