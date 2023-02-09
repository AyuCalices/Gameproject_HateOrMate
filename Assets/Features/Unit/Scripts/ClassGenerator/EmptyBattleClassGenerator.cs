using Features.Battle.Scripts;
using Features.Loot.Scripts.ModView;
using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Class;
using Features.Unit.Scripts.DamageAnimation;
using Features.Unit.Scripts.View;
using UnityEngine;

namespace Features.Unit.Scripts.ClassGenerator
{
    [CreateAssetMenu(fileName = "new EmptyBattleActions", menuName = "Unit/Actions/EmptyBattleActions")]
    public class EmptyBattleClassGenerator : BattleClassGenerator_SO
    {
        [SerializeField] private DamagePopup damagePopupPrefab;
        [SerializeField] private CanvasFocus_SO canvasFocus;
        
        protected override BattleClass InternalGenerate(BaseDamageAnimationBehaviour baseDamageAnimationBehaviour, NetworkedStatsBehaviour ownerNetworkingStatsBehaviour, BattleBehaviour ownerBattleBehaviour,
            UnitBattleView ownerUnitBattleView)
        {
            return new EmptyBattleClass(ownerNetworkingStatsBehaviour, ownerBattleBehaviour, ownerUnitBattleView, damagePopupPrefab, canvasFocus);
        }
    }
}
