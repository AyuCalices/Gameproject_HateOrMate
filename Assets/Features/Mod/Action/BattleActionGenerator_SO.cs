using Features.GlobalReferences;
using Features.GlobalReferences.Scripts;
using Features.Unit.Battle;
using Features.Unit.Battle.Scripts;
using Features.Unit.Battle.Scripts.Actions;
using Features.Unit.Modding;
using Features.Unit.View;
using UnityEngine;

namespace Features.Mod.Action
{
    public abstract class BattleActionGenerator_SO : ScriptableObject
    {
        public BattleActions Generate(NetworkedStatsBehaviour ownerNetworkingStatsBehaviour, BattleBehaviour ownerBattleBehaviour,
            UnitView ownerUnitView)
        {
            return InternalGenerate(ownerNetworkingStatsBehaviour, ownerBattleBehaviour, ownerUnitView);
        }

        protected abstract BattleActions InternalGenerate(NetworkedStatsBehaviour ownerNetworkingStatsBehaviour, BattleBehaviour ownerBattleBehaviour,
            UnitView ownerUnitView);
    }
}
