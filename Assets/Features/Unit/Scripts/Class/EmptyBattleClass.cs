using Features.Battle.Scripts;
using Features.Battle.Scripts.Unit.ServiceLocatorSystem;
using Features.Loot.Scripts.ModView;
using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.View;

namespace Features.Unit.Scripts.Class
{
    public class EmptyBattleClass : BattleClass
    {
        public EmptyBattleClass(UnitServiceProvider ownerUnitServiceProvider) : base(ownerUnitServiceProvider)
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
