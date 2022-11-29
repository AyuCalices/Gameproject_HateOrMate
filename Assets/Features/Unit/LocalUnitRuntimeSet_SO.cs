using DataStructures.RuntimeSet;
using Features.Mod;
using Features.ModView;
using UnityEngine;

namespace Features.Unit
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
    }
}
