using System.Collections.Generic;
using DataStructures.RuntimeSet;
using Features.Mod;
using Features.ModView;
using Features.Unit.Battle.Scripts;
using Features.Unit.Battle.Scripts.Actions;
using Features.Unit.Modding;
using Photon.Pun;
using UnityEngine;

namespace Features.GlobalReferences.Scripts
{
    public enum UnitControlType { Master, Client, AI }
    
    [CreateAssetMenu(fileName = "new NetworkedUnitRuntimeSet", menuName = "Unit/Networked RuntimeSet")]
    public class NetworkedUnitRuntimeSet_SO : RuntimeSet_SO<NetworkedStatsBehaviour>
    {
        public bool TryInstantiateModToAny(ModDragBehaviour modDragBehaviour, BaseMod baseMod)
        {
            foreach (NetworkedStatsBehaviour localUnitBehaviour in GetItems())
            {
                //TODO: getComponent
                if (!localUnitBehaviour.TryGetComponent(out ModUnitBehaviour modUnitBehaviour)) continue;
                if (modUnitBehaviour.UnitMods.TryInstantiateMod(modDragBehaviour, baseMod))
                {
                    return true;
                }
            }

            return false;
        }

        private bool ContainsTargetable(out List<NetworkedStatsBehaviour> networkedUnitBehaviours)
        {
            networkedUnitBehaviours = new List<NetworkedStatsBehaviour>();
            
            //parse for targetable
            var list = GetItems();
            for (int i = list.Count - 1; i >= 0; i--)
            {
                //TODO: getComponent
                if (list[i].TryGetComponent(out NetworkedBattleBehaviour battleBehaviour) && battleBehaviour.IsTargetable && battleBehaviour.CurrentState is not DeathState)
                {
                    networkedUnitBehaviours.Add(list[i]);
                }
            }

            return networkedUnitBehaviours.Count > 0;
        }

        public bool TryGetClosestTargetableByWorldPosition(Vector3 worldPosition, out KeyValuePair<NetworkedStatsBehaviour, float> closestUnit)
        {
            if (!ContainsTargetable(out List<NetworkedStatsBehaviour> networkedUnitBehaviours))
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

            closestUnit = new KeyValuePair<NetworkedStatsBehaviour, float>(networkedUnitBehaviours[closestUnitIndex], closestDistance);
            return true;
        }

        public bool HasUnitAlive()
        {
            foreach (NetworkedStatsBehaviour networkedUnitBehaviour in GetItems())
            {
                //TODO: getComponent
                if (networkedUnitBehaviour.TryGetComponent(out NetworkedBattleBehaviour battleBehaviour))
                {
                    if (battleBehaviour.CurrentState is not DeathState && battleBehaviour.IsTargetable)
                    {
                        return true;
                    }
                    
                }
            }

            return false;
        }

        public bool TryGetUnitByViewID(int requestedViewID, out NetworkedStatsBehaviour networkedStatsBehaviour)
        {
            foreach (NetworkedStatsBehaviour item in items)
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
