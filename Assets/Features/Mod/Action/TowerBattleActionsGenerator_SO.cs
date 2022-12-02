using Features.GlobalReferences;
using Features.Unit.Battle;
using Features.Unit.Modding;
using UnityEngine;

namespace Features.Mod.Action
{
    [CreateAssetMenu(fileName = "new TowerBattleActions", menuName = "Unit/Actions/TowerBattleActions")]
    public class TowerBattleActionsGenerator_SO : BattleActionGenerator_SO
    {
        public override BattleActions Generate(NetworkedUnitBehaviour ownerNetworkingUnitBehaviour, BattleBehaviour ownerBattleBehaviour,
            UnitView ownerUnitView, NetworkedUnitRuntimeSet_SO opponentNetworkedUnitRuntimeSet)
        {
            return new TowerBattleActions(ownerNetworkingUnitBehaviour, ownerBattleBehaviour, ownerUnitView,
                opponentNetworkedUnitRuntimeSet, 10, 10);
        }
    }
}
