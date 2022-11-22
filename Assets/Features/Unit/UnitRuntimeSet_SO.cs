using DataStructures.RuntimeSet;
using UnityEngine;

namespace Features.Unit
{
    [CreateAssetMenu(fileName = "new UnitRuntimeSet", menuName = "Unit/Runtime Set")]
    public class UnitRuntimeSet_SO : RuntimeSet_SO<UnitBehaviour>
    {
        public bool TryGetByIdentity(string identity, out UnitBehaviour unit)
        {
            bool result = GetItems().Exists(x => x.Identity == identity);
            if (result)
            {
                unit = GetItems().Find(x => x.Identity == identity);
            }
            
            unit = null;
            return result;
        }
    }
}
