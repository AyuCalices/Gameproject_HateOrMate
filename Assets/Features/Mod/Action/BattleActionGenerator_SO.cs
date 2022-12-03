using Features.GlobalReferences;
using Features.Unit.Battle;
using Features.Unit.Battle.Actions;
using Features.Unit.Modding;
using Features.Unit.View;
using UnityEngine;

namespace Features.Mod.Action
{
    public abstract class BattleActionGenerator_SO : ScriptableObject
    {
        public BattleActions Generate(NetworkedUnitBehaviour ownerNetworkingUnitBehaviour, BattleBehaviour ownerBattleBehaviour,
            UnitView ownerUnitView)
        {
            return InternalGenerate(ownerNetworkingUnitBehaviour, ownerBattleBehaviour, ownerUnitView,
                ownerNetworkingUnitBehaviour.EnemyRuntimeSet);
        }

        protected abstract BattleActions InternalGenerate(NetworkedUnitBehaviour ownerNetworkingUnitBehaviour, BattleBehaviour ownerBattleBehaviour,
            UnitView ownerUnitView, NetworkedUnitRuntimeSet_SO opponentNetworkedUnitRuntimeSet);
    }
}
