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
    [CreateAssetMenu(fileName = "new EmptyBattleActions", menuName = "Unit/Actions/EmptyBattleActions")]
    public class EmptyBattleActionsGenerator_SO : BattleActionGenerator_SO
    {
        protected override BattleActions InternalGenerate(NetworkedStatsBehaviour ownerNetworkingStatsBehaviour, BattleBehaviour ownerBattleBehaviour,
            UnitView ownerUnitView)
        {
            return new EmptyBattleActions(ownerNetworkingStatsBehaviour, ownerBattleBehaviour, ownerUnitView);
        }
    }
}
