using System;
using System.Collections.Generic;
using System.Linq;
using DataStructures.RuntimeSet;
using Features.Unit.Scripts.Behaviours.Services.BattleBehaviour;
using Photon.Pun;
using UnityEngine;

namespace Features.Unit.Scripts.Behaviours
{
    
    [CreateAssetMenu(fileName = "new NetworkedUnitRuntimeSet", menuName = "Unit/Networked RuntimeSet")]
    public class UnitServiceProviderRuntimeSet_SO : RuntimeSet_SO<UnitServiceProvider>
    {
        private void OnEnable()
        {
            Restore();
        }

        public List<UnitServiceProvider> GetUnitsByTag(params TeamTagType[] teamTagTypes)
        {
            List<UnitServiceProvider> foundUnits = new List<UnitServiceProvider>();

            foreach (UnitServiceProvider unitServiceProvider in items)
            {
                foreach (var teamTagType in teamTagTypes)
                {
                    if (unitServiceProvider.TeamTagTypes.Any(e => e == teamTagType))
                    {
                        foundUnits.Add(unitServiceProvider);
                    }
                }
            }

            return foundUnits;
        }

        public bool GetUnitByViewID(int requestedViewID, out UnitServiceProvider requestedUnitServiceProvider)
        {
            foreach (UnitServiceProvider item in items)
            {
                if (requestedViewID == item.GetService<PhotonView>().ViewID)
                {
                    requestedUnitServiceProvider = item;
                    return true;
                }
            }

            requestedUnitServiceProvider = default;
            return false;
        }
    }
}
