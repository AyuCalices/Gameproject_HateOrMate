using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Behaviours.Stat;
using Features.Unit.Scripts.Class;
using Features.Unit.Scripts.View;
using UnityEngine;

namespace Features.Unit.Scripts.ClassGenerator
{
    [CreateAssetMenu(fileName = "new TowerBattleActions", menuName = "Unit/Actions/TowerBattleActions")]
    public class TowerBattleClassGenerator_SO : BattleClassGenerator_SO
    {
        [SerializeField] private ProjectileDamageAnimationBehaviour projectileDamageAnimationBehaviour;
        [SerializeField] private float towerDamageMultiplier = 0.5f;
        
        protected override BattleClass InternalGenerate(NetworkedStatsBehaviour ownerNetworkingStatsBehaviour, BattleBehaviour ownerBattleBehaviour,
            UnitBattleView ownerUnitBattleView)
        {
            return new TowerBattleClass(ownerNetworkingStatsBehaviour, ownerBattleBehaviour, ownerUnitBattleView, projectileDamageAnimationBehaviour, towerDamageMultiplier);
        }
    }
}
