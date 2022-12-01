using System.Collections.Generic;
using DataStructures.RuntimeSet;
using Features.ModView;
using Features.Unit;
using Features.Unit.Battle;
using Features.Unit.Modding;
using UnityEngine;

namespace Features.GlobalReferences
{
    [CreateAssetMenu(fileName = "new LocalUnitRuntimeSet", menuName = "Unit/Local RuntimeSet")]
    public class LocalUnitRuntimeSet_SO : RuntimeSet_SO<LocalUnitBehaviour>
    {
        public bool TryAddModToAny(ModDragBehaviour modDragBehaviour)
        {
            foreach (LocalUnitBehaviour localUnitBehaviour in GetItems())
            {
                if (localUnitBehaviour.UnitMods.TryAddMod(modDragBehaviour))
                {
                    return true;
                }
            }

            return false;
        }
        
        public bool TryGetByIdentity(int identity, out LocalUnitBehaviour localUnit)
        {
            bool result = GetItems().Exists(x => x.ViewID == identity);
            if (result)
            {
                localUnit = GetItems().Find(x => x.ViewID == identity);
            }
            
            localUnit = null;
            return result;
        }

        public List<T> GetAllUnitsByBattleAction<T>() where T : BattleActions
        {
            List<T> foundUnits = new List<T>();

            List<LocalUnitBehaviour> list = GetItems();

            foreach (LocalUnitBehaviour item in list)
            {
                if (item.GetComponent<BattleBehaviour>().BattleActions is T action)
                {
                    foundUnits.Add(action);
                }
            }

            return foundUnits;
        }
    }
}
