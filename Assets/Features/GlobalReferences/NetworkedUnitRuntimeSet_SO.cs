using DataStructures.RuntimeSet;
using Features.Unit;
using Features.Unit.Modding;
using UnityEngine;

namespace Features.GlobalReferences
{
    [CreateAssetMenu(fileName = "new NetworkedUnitRuntimeSet", menuName = "Unit/Networked RuntimeSet")]
    public class NetworkedUnitRuntimeSet_SO : RuntimeSet_SO<NetworkedUnitBehaviour>
    {
        public bool TryGetByIdentity(int identity, out NetworkedUnitBehaviour localUnit)
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
