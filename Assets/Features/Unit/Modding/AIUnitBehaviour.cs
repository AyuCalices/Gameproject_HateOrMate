using Features.GlobalReferences;
using Features.GlobalReferences.Scripts;

namespace Features.Unit.Modding
{
    public class AIUnitBehaviour : NetworkedUnitBehaviour
    {
        protected override void InternalAwake()
        {
            OwnerNetworkedPlayerUnits.Add(this);
            EnemyRuntimeSet = battleData.PlayerTeamUnitRuntimeSet;
        
            OnPhotonViewIdAllocated();
        }

        protected override void InternalOnNetworkingEnabled()
        {
            battleData.SetAiStats(this);
        }

        protected override void InternalOnDestroy()
        {
            OwnerNetworkedPlayerUnits.Remove(this);
        }
    }
}
