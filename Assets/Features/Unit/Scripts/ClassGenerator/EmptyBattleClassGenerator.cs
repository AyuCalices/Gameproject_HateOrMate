using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Class;
using Features.Unit.Scripts.DamageAnimation;
using UnityEngine;

namespace Features.Unit.Scripts.ClassGenerator
{
    [CreateAssetMenu(fileName = "new EmptyBattleActions", menuName = "Unit/Actions/EmptyBattleActions")]
    public class EmptyBattleClassGenerator : BattleClassGenerator_SO
    {
        protected override BattleClass InternalGenerate(UnitServiceProvider ownerUnitServiceProvider, BaseDamageAnimationBehaviour baseDamageAnimationBehaviour)
        {
            return new EmptyBattleClass(ownerUnitServiceProvider);
        }
    }
}
