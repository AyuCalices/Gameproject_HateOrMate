using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Class;
using Features.Unit.Scripts.DamageAnimation;
using Features.Unit.Scripts.View;
using UnityEngine;

namespace Features.Unit.Scripts.ClassGenerator
{
    [CreateAssetMenu(fileName = "new TowerBattleActions", menuName = "Unit/Actions/TowerBattleActions")]
    public class TowerBattleClassGenerator_SO : BattleClassGenerator_SO
    {
        protected override BattleClass InternalGenerate(BaseDamageAnimationBehaviour baseDamageAnimationBehaviour, NetworkedStatsBehaviour ownerNetworkingStatsBehaviour, 
            NetworkedBattleBehaviour ownerBattleBehaviour, UnitBattleView ownerUnitBattleView)
        {
            return new TowerBattleClass(ownerNetworkingStatsBehaviour, ownerBattleBehaviour, ownerUnitBattleView, baseDamageAnimationBehaviour);
        }
    }
}
