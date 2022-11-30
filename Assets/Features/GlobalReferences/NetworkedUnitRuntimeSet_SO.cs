using System.Collections.Generic;
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

        public KeyValuePair<NetworkedUnitBehaviour, float> GetClosestByWorldPosition(Vector3 worldPosition)
        {
            var list = GetItems();
            
            int closestUnitIndex = 0;
            float closestDistance = Vector3.Distance(worldPosition, list[0].transform.position);
            
            for (int index = 1; index < list.Count; index++)
            {
                float distanceNext = Vector3.Distance(worldPosition, list[index].transform.position);
                if (distanceNext < closestDistance)
                {
                    closestUnitIndex = index;
                    closestDistance = distanceNext;
                }
            }

            return new KeyValuePair<NetworkedUnitBehaviour, float>(list[closestUnitIndex], closestDistance);
        }

        public bool IsInRangeByWorldPosition(float range, Vector3 worldPosition)
        {
            var list = GetItems();
            foreach (NetworkedUnitBehaviour networkedUnitBehaviour in list)
            {
                float distanceNext = Vector3.Distance(worldPosition, networkedUnitBehaviour.transform.position);
                if (distanceNext < range)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
