using Features.GlobalReferences;
using Features.Unit.Modding;
using Features.Unit.Modding.Stat;
using Photon.Pun;

public class AIUnitBehaviour : NetworkedUnitBehaviour
{
    protected override void InternalAwake()
    {
        ControlType = UnitControlType.AI;
        OwnerNetworkedPlayerUnits.Add(this);
        EnemyRuntimeSet = battleData.PlayerTeamUnitRuntimeSet;
        
        OnPhotonViewIdAllocated();
    }

    protected override void InternalOnNetworkingEnabled()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            NetworkedStatServiceLocator.TryAddLocalValue(StatType.Damage, StatValueType.Stat, 10);
            NetworkedStatServiceLocator.TryAddLocalValue(StatType.Damage, StatValueType.ScalingStat, 1);
            
            NetworkedStatServiceLocator.TryAddLocalValue(StatType.Health, StatValueType.Stat, 50);
            NetworkedStatServiceLocator.TryAddLocalValue(StatType.Health, StatValueType.ScalingStat, 1);
            
            NetworkedStatServiceLocator.TryAddLocalValue(StatType.Speed, StatValueType.Stat, 3);
            NetworkedStatServiceLocator.TryAddLocalValue(StatType.Speed, StatValueType.ScalingStat, 1);
        }
    }

    protected override void InternalOnDestroy()
    {
        OwnerNetworkedPlayerUnits.Remove(this);
    }
}
