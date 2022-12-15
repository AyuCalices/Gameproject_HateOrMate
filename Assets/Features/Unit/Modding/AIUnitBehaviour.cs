using Features.GlobalReferences;
using Features.GlobalReferences.Scripts;

namespace Features.Unit.Modding
{
    public class AIUnitBehaviour : NetworkedUnitBehaviour
    {
        protected override void InternalAwake()
        {
            ownerNetworkedPlayerUnits.Add(this);
            EnemyRuntimeSet = battleData.PlayerUnitsRuntimeSet;
        
            OnPhotonViewIdAllocated();
        }

        protected override void InternalOnNetworkingEnabled()
        {
            battleData.SetAiStats(this);
        }

        protected override void InternalOnDestroy()
        {
            ownerNetworkedPlayerUnits.Remove(this);
        }
    }
}
