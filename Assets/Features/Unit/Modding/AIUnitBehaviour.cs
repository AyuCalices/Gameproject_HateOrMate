using Features.GlobalReferences;

namespace Features.Unit.Modding
{
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
            battleData.SetAiStats(this);
        }

        protected override void InternalOnDestroy()
        {
            OwnerNetworkedPlayerUnits.Remove(this);
        }
    }
}
