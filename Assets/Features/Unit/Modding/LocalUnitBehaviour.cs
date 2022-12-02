using Features.GlobalReferences;
using Features.Unit.Modding.Stat;
using Photon.Pun;
using UnityEngine;

namespace Features.Unit.Modding
{
    //TODO: implement base stat values
    public class LocalUnitBehaviour : NetworkedUnitBehaviour
    {
        protected override void InternalAwake()
        {
            EnemyRuntimeSet = battleData.EnemyUnitRuntimeSet;
            
            OwnerNetworkedPlayerUnits.Add(this);
            battleData.PlayerTeamUnitRuntimeSet.Add(this);
            
            if (PhotonNetwork.IsMasterClient)
            {
                ControlType = UnitControlType.Master;
            }
            else
            {
                ControlType = UnitControlType.Client;
            }
        }

        protected override void InternalOnNetworkingEnabled()
        {
            NetworkedStatServiceLocator.TryAddLocalValue(StatType.Damage, StatValueType.Stat, 10);
            NetworkedStatServiceLocator.TryAddLocalValue(StatType.Damage, StatValueType.ScalingStat, 1);
            
            NetworkedStatServiceLocator.TryAddLocalValue(StatType.Health, StatValueType.Stat, 50);
            NetworkedStatServiceLocator.TryAddLocalValue(StatType.Health, StatValueType.ScalingStat, 1);
            
            NetworkedStatServiceLocator.TryAddLocalValue(StatType.Speed, StatValueType.Stat, 3);
            NetworkedStatServiceLocator.TryAddLocalValue(StatType.Speed, StatValueType.ScalingStat, 1);
        }

        protected override void InternalOnDestroy()
        {
            OwnerNetworkedPlayerUnits.Remove(this);
            battleData.PlayerTeamUnitRuntimeSet.Remove(this);
        }
    }
}
