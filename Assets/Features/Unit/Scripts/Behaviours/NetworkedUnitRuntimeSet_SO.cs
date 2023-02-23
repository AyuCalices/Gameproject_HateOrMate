using System;
using System.Collections.Generic;
using System.Linq;
using DataStructures.RuntimeSet;
using Features.Unit.Scripts.Behaviours.Battle;
using UnityEngine;

namespace Features.Unit.Scripts.Behaviours
{
    
    [CreateAssetMenu(fileName = "new NetworkedUnitRuntimeSet", menuName = "Unit/Networked RuntimeSet")]
    public class NetworkedUnitRuntimeSet_SO : RuntimeSet_SO<NetworkedBattleBehaviour>
    {
        private void OnEnable()
        {
            Restore();
        }

        public List<NetworkedBattleBehaviour> GetUnitsByTag(params TeamTagType[] teamTagTypes)
        {
            List<NetworkedBattleBehaviour> foundUnits = new List<NetworkedBattleBehaviour>();

            foreach (NetworkedBattleBehaviour networkedBattleBehaviour in items)
            {
                foreach (var teamTagType in teamTagTypes)
                {
                    if (networkedBattleBehaviour.TeamTagTypes.Any(e => e == teamTagType))
                    {
                        foundUnits.Add(networkedBattleBehaviour);
                    }
                }
            }

            return foundUnits;
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
