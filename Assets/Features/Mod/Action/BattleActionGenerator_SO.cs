using Features.GlobalReferences;
using Features.Unit.Battle;
using Features.Unit.Modding;
using UnityEngine;

namespace Features.Mod.Action
{
    public abstract class BattleActionGenerator_SO : ScriptableObject
    {
        public abstract BattleActions Generate(NetworkedUnitBehaviour ownerNetworkingUnitBehaviour, BattleBehaviour ownerBattleBehaviour,
            UnitView ownerUnitView, NetworkedUnitRuntimeSet_SO opponentNetworkedUnitRuntimeSet);
    }
}
