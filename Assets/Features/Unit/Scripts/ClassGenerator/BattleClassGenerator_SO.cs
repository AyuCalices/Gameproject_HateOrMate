using Features.Battle.Scripts.Unit.ServiceLocatorSystem;
using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Class;
using Features.Unit.Scripts.DamageAnimation;
using Features.Unit.Scripts.View;
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
