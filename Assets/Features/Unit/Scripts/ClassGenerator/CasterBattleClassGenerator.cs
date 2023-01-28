using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Behaviours.Stat;
using Features.Unit.Scripts.Class;
using Features.Unit.Scripts.View;
using UnityEngine;

namespace Features.Unit.Scripts.ClassGenerator
{
    [CreateAssetMenu(fileName = "new AiTowerBattleActions", menuName = "Unit/Actions/AiTowerBattleActions")]
    public class CasterBattleClassGenerator : BattleClassGenerator_SO
    {
        [SerializeField] private SlashDamageAnimationBehaviour projectileDamageAnimationBehaviour;
        [SerializeField] private bool isAI;
        
        protected override BattleClass InternalGenerate(NetworkedStatsBehaviour ownerNetworkingStatsBehaviour, BattleBehaviour ownerBattleBehaviour,
            UnitBattleView ownerUnitBattleView)
        {
            return new CasterBattleClass(isAI, ownerNetworkingStatsBehaviour, ownerBattleBehaviour, ownerUnitBattleView, projectileDamageAnimationBehaviour);
        }
    }
}
