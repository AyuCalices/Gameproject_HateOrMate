using Features.GlobalReferences;
using Features.Unit.Battle;
using Features.Unit.Modding;
using UnityEngine;

namespace Features.Mod.Action
{
    [CreateAssetMenu(fileName = "new EmptyBattleActions", menuName = "Unit/Actions/EmptyBattleActions")]
    public class EmptyBattleActionsGenerator_SO : BattleActionGenerator_SO
    {
        public override BattleActions Generate(NetworkedUnitBehaviour ownerNetworkingUnitBehaviour, BattleBehaviour ownerBattleBehaviour,
            UnitView ownerUnitView, NetworkedUnitRuntimeSet_SO opponentNetworkedUnitRuntimeSet)
        {
            return new EmptyBattleActions(ownerNetworkingUnitBehaviour, ownerBattleBehaviour, ownerUnitView,
                opponentNetworkedUnitRuntimeSet);
        }
    }
}
