using Features.GlobalReferences;
using Features.Unit.Modding;

namespace Features.Unit.Battle
{
    public class EmptyBattleActions : BattleActions
    {
        public EmptyBattleActions(NetworkedUnitBehaviour ownerNetworkingUnitBehaviour, BattleBehaviour ownerBattleBehaviour, UnitView ownerUnitView, NetworkedUnitRuntimeSet_SO opponentNetworkedUnitRuntimeSet) : base(ownerNetworkingUnitBehaviour, ownerBattleBehaviour, ownerUnitView, opponentNetworkedUnitRuntimeSet)
        {
        }

        protected override void InternalUpdateBattleActions()
        {
        }

        protected override void InternalOnPerformAction()
        {
        }
    }
}
