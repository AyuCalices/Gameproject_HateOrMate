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
    public class NetworkedUnitRuntimeSet_SO : RuntimeSet_SO<NetworkedUnitBehaviour>
    {
        public bool TryInstantiateModToAny(ModDragBehaviour modDragBehaviour, BaseMod baseMod)
        {
            foreach (NetworkedUnitBehaviour localUnitBehaviour in GetItems())
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

        private bool ContainsTargetable(out List<NetworkedUnitBehaviour> networkedUnitBehaviours)
        {
            networkedUnitBehaviours = new List<NetworkedUnitBehaviour>();
            
            //parse for targetable
            var list = GetItems();
            for (int i = list.Count - 1; i >= 0; i--)
            {
                //TODO: getComponent
                if (list[i].TryGetComponent(out BattleBehaviour battleBehaviour) && battleBehaviour.IsTargetable && battleBehaviour.CurrentState is not DeathState)
                {
                    networkedUnitBehaviours.Add(list[i]);
                }
            }

            return networkedUnitBehaviours.Count > 0;
        }

        public bool TryGetClosestTargetableByWorldPosition(Vector3 worldPosition, out KeyValuePair<NetworkedUnitBehaviour, float> closestUnit)
        {
            if (!ContainsTargetable(out List<NetworkedUnitBehaviour> networkedUnitBehaviours))
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

            closestUnit = new KeyValuePair<NetworkedUnitBehaviour, float>(networkedUnitBehaviours[closestUnitIndex], closestDistance);
            return true;
        }

        public bool HasUnitAlive()
        {
            foreach (NetworkedUnitBehaviour networkedUnitBehaviour in GetItems())
            {
                //TODO: getComponent
                if (networkedUnitBehaviour.TryGetComponent(out BattleBehaviour battleBehaviour))
                {
                    if (battleBehaviour.CurrentState is not DeathState && battleBehaviour.IsTargetable)
                    {
                        return true;
                    }
                    
                }
            }

            return false;
        }

        public bool TryGetUnitByViewID(int requestedViewID, out NetworkedUnitBehaviour networkedUnitBehaviour)
        {
            foreach (NetworkedUnitBehaviour item in items)
            {
                if (requestedViewID == item.PhotonView.ViewID)
                {
                    networkedUnitBehaviour = item;
                    return true;
                }
            }

            networkedUnitBehaviour = default;
            return false;
        }
    }
}
