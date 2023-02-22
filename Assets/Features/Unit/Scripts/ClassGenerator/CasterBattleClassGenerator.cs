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
        
        protected override BattleClass InternalGenerate(BaseDamageAnimationBehaviour baseDamageAnimationBehaviour, NetworkedStatsBehaviour ownerNetworkingStatsBehaviour, 
            NetworkedBattleBehaviour ownerBattleBehaviour, UnitBattleView ownerUnitBattleView)
        {
            return new CasterBattleClass(isAI, ownerNetworkingStatsBehaviour, ownerBattleBehaviour, ownerUnitBattleView, baseDamageAnimationBehaviour);
        }
    }
}
