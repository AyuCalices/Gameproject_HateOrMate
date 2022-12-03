using Features.GlobalReferences;
using Features.Unit.Battle;
using Features.Unit.Battle.Actions;
using Features.Unit.Modding;
using Features.Unit.View;
using UnityEngine;

namespace Features.Mod.Action
{
    [CreateAssetMenu(fileName = "new EmptyBattleActions", menuName = "Unit/Actions/EmptyBattleActions")]
    public class EmptyBattleActionsGenerator_SO : BattleActionGenerator_SO
    {
        protected override BattleActions InternalGenerate(NetworkedUnitBehaviour ownerNetworkingUnitBehaviour, BattleBehaviour ownerBattleBehaviour,
            UnitView ownerUnitView, NetworkedUnitRuntimeSet_SO opponentNetworkedUnitRuntimeSet)
        {
            return new EmptyBattleActions(ownerNetworkingUnitBehaviour, ownerBattleBehaviour, ownerUnitView,
                opponentNetworkedUnitRuntimeSet);
        }
    }
}
