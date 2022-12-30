using System.Collections.Generic;
using System.Linq;
using DataStructures.RuntimeSet;
using Features.Loot.Scripts.GeneratedLoot;
using Features.Loot.Scripts.ModView;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Behaviours.Mod;
using UnityEngine;

namespace Features.Unit.Scripts.Behaviours
{
    
    [CreateAssetMenu(fileName = "new NetworkedUnitRuntimeSet", menuName = "Unit/Networked RuntimeSet")]
    public class NetworkedUnitRuntimeSet_SO : RuntimeSet_SO<NetworkedBattleBehaviour>
    {
        //battle
        private bool ContainsTargetable(out List<NetworkedBattleBehaviour> networkedUnitBehaviours)
        {
            networkedUnitBehaviours = items.Where(t => t.IsTargetable && t.CurrentState is not DeathState).ToList();

            return networkedUnitBehaviours.Count > 0;
        }

        //battle
        public bool TryGetClosestTargetableByWorldPosition(Vector3 worldPosition, out KeyValuePair<NetworkedBattleBehaviour, float> closestUnit)
        {
            if (!ContainsTargetable(out List<NetworkedBattleBehaviour> networkedUnitBehaviours))
            {
                closestUnit = default;
                return false;
            }

            //get closest
            int closestUnitIndex = 0;
            float closestDistance = Vector3.Distance(worldPosition, networkedUnitBehaviours[0].transform.position);
            
            for (int index = 1; index < networkedUnitBehaviours.Count; index++)
            {
                float distanceNext = Vector3.Distance(worldPosition, networkedUnitBehaviours[index].transform.position);
                if (distanceNext < closestDistance)
                {
                    closestUnitIndex = index;
                    closestDistance = distanceNext;
                }
            }

            closestUnit = new KeyValuePair<NetworkedBattleBehaviour, float>(networkedUnitBehaviours[closestUnitIndex], closestDistance);
            return true;
        }
        
        //battle
        public bool HasUnitAlive()
        {
            return GetItems().Any(networkedUnitBehaviour => networkedUnitBehaviour.CurrentState is not DeathState && networkedUnitBehaviour.IsTargetable);
        }

        public bool TryGetUnitByViewID(int requestedViewID, out NetworkedBattleBehaviour networkedStatsBehaviour)
        {
            foreach (NetworkedBattleBehaviour item in items)
            {
                if (requestedViewID == item.PhotonView.ViewID)
                {
                    networkedStatsBehaviour = item;
                    return true;
                }
            }

            networkedStatsBehaviour = default;
            return false;
        }
    }
}
