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
            EnemyRuntimeSet = battleData.EnemyUnitsRuntimeSet;
            
            ownerNetworkedPlayerUnits.Add(this);
            battleData.PlayerUnitsRuntimeSet.Add(this);
        }

        protected override void InternalOnDestroy()
        {
            ownerNetworkedPlayerUnits.Remove(this);
            battleData.PlayerUnitsRuntimeSet.Remove(this);
        }
    }
}
