using Features.Battle.Scripts;
using Features.Loot.Scripts.ModView;
using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.View;

namespace Features.Unit.Scripts.Class
{
    public class EmptyBattleClass : BattleClass
    {
        public EmptyBattleClass(NetworkedStatsBehaviour ownerNetworkingStatsBehaviour, NetworkedBattleBehaviour ownerBattleBehaviour, 
            UnitBattleView ownerUnitBattleView) : 
            base(ownerNetworkingStatsBehaviour, ownerBattleBehaviour, ownerUnitBattleView)
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
