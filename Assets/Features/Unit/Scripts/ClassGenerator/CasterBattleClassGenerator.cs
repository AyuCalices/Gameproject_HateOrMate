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
    [CreateAssetMenu(fileName = "new AiTowerBattleActions", menuName = "Unit/Actions/AiTowerBattleActions")]
    public class CasterBattleClassGenerator : BattleClassGenerator_SO
    {
        [SerializeField] private bool isAI;
        [SerializeField] private DamagePopup damagePopupPrefab;
        [SerializeField] private CanvasFocus_SO canvasFocus;
        
        protected override BattleClass InternalGenerate(BaseDamageAnimationBehaviour baseDamageAnimationBehaviour, NetworkedStatsBehaviour ownerNetworkingStatsBehaviour, 
            BattleBehaviour ownerBattleBehaviour, UnitBattleView ownerUnitBattleView)
        {
            return new CasterBattleClass(isAI, ownerNetworkingStatsBehaviour, ownerBattleBehaviour, ownerUnitBattleView, baseDamageAnimationBehaviour, damagePopupPrefab, canvasFocus);
        }
    }
}
