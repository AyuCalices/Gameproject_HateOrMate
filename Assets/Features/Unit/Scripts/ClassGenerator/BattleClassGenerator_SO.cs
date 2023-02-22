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
        public BattleClass Generate(BaseDamageAnimationBehaviour baseDamageAnimationBehaviour, NetworkedStatsBehaviour ownerNetworkingStatsBehaviour, 
            NetworkedBattleBehaviour ownerBattleBehaviour, UnitBattleView ownerUnitBattleView)
        {
            return InternalGenerate(baseDamageAnimationBehaviour, ownerNetworkingStatsBehaviour, ownerBattleBehaviour, ownerUnitBattleView);
        }

        protected abstract BattleClass InternalGenerate(BaseDamageAnimationBehaviour baseDamageAnimationBehaviour, NetworkedStatsBehaviour ownerNetworkingStatsBehaviour, 
            NetworkedBattleBehaviour ownerBattleBehaviour, UnitBattleView ownerUnitBattleView);
    }
}
