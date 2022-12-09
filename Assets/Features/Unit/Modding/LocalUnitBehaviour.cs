using Features.GlobalReferences;
using Features.GlobalReferences.Scripts;
using Features.Unit.Modding.Stat;
using Photon.Pun;

namespace Features.Unit.Modding
{
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

        protected override void InternalOnDestroy()
        {
            OwnerNetworkedPlayerUnits.Remove(this);
            battleData.PlayerTeamUnitRuntimeSet.Remove(this);
        }
    }
}
