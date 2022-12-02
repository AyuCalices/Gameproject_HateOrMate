using Features.GlobalReferences;
using Features.Unit.Battle;
using Features.Unit.Modding;
using UnityEngine;

namespace Features.Mod.Action
{
    [CreateAssetMenu(fileName = "new AiTowerBattleActions", menuName = "Unit/Actions/AiTowerBattleActions")]
    public class AiTowerBattleActionsGenerator_SO : BattleActionGenerator_SO
    {
        public override BattleActions Generate(NetworkedUnitBehaviour ownerNetworkingUnitBehaviour, BattleBehaviour ownerBattleBehaviour,
            UnitView ownerUnitView, NetworkedUnitRuntimeSet_SO opponentNetworkedUnitRuntimeSet)
        {
            return new AiTowerBattleActions(ownerNetworkingUnitBehaviour, ownerBattleBehaviour, ownerUnitView,
                opponentNetworkedUnitRuntimeSet);
        }
    }
}
