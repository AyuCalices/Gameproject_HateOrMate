using DataStructures.RuntimeSet;
using UnityEngine;

namespace Features.Unit
{
    [CreateAssetMenu(fileName = "new LocalUnitRuntimeSet", menuName = "Unit/Local RuntimeSet")]
    public class LocalUnitRuntimeSet_SO : RuntimeSet_SO<LocalUnitBehaviour>
    {
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
