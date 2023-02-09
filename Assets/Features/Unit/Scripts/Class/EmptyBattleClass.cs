using Features.Battle.Scripts;
using Features.Loot.Scripts.ModView;
using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.View;

namespace Features.Unit.Scripts.Class
{
    public class EmptyBattleClass : BattleClass
    {
        public EmptyBattleClass(NetworkedStatsBehaviour ownerNetworkingStatsBehaviour, BattleBehaviour ownerBattleBehaviour, 
            UnitBattleView ownerUnitBattleView, DamagePopup damagePopupPrefab, CanvasFocus_SO canvasFocus) : 
            base(ownerNetworkingStatsBehaviour, ownerBattleBehaviour, ownerUnitBattleView, damagePopupPrefab, canvasFocus)
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
