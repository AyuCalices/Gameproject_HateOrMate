using Features.GlobalReferences.Scripts;
using Features.Unit.Modding;
using Features.Unit.View;

namespace Features.Unit.Battle.Scripts.Actions
{
    public class EmptyBattleActions : BattleActions
    {
        public EmptyBattleActions(NetworkedUnitBehaviour ownerNetworkingUnitBehaviour, BattleBehaviour ownerBattleBehaviour, 
            UnitView ownerUnitView) : base(ownerNetworkingUnitBehaviour, ownerBattleBehaviour, ownerUnitView)
        {
        }

        protected override void InternalInitializeBattleActions()
        {
        }

        protected override void InternalUpdateBattleActions()
        {
        }

        public override void OnStageEnd()
        {
        }

        protected override void InternalOnPerformAction()
        {
        }
    }
}
