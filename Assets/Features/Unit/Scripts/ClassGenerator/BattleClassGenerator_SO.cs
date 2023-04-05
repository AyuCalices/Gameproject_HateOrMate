using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Class;
using Features.Unit.Scripts.DamageAnimation;
using UnityEngine;

namespace Features.Unit.Scripts.ClassGenerator
{
    public abstract class BattleClassGenerator_SO : ScriptableObject
    {
        public BattleClass Generate(UnitServiceProvider ownerUnitServiceProvider, BaseDamageAnimationBehaviour baseDamageAnimationBehaviour)
        {
            return InternalGenerate(ownerUnitServiceProvider, baseDamageAnimationBehaviour);
        }

        protected abstract BattleClass InternalGenerate(UnitServiceProvider ownerUnitServiceProvider, BaseDamageAnimationBehaviour baseDamageAnimationBehaviour);
    }
}
