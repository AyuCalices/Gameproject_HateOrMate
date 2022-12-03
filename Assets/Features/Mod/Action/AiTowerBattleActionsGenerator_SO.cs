using Features.Battle;
using Features.GlobalReferences;
using Features.Unit.Battle;
using Features.Unit.Battle.Actions;
using Features.Unit.Modding;
using Features.Unit.View;
using UnityEngine;

namespace Features.Mod.Action
{
    [CreateAssetMenu(fileName = "new AiTowerBattleActions", menuName = "Unit/Actions/AiTowerBattleActions")]
    public class AiTowerBattleActionsGenerator_SO : BattleActionGenerator_SO
    {
        [SerializeField] private DamageProjectileBehaviour damageProjectileBehaviour;
        
        protected override BattleActions InternalGenerate(NetworkedUnitBehaviour ownerNetworkingUnitBehaviour, BattleBehaviour ownerBattleBehaviour,
            UnitView ownerUnitView, NetworkedUnitRuntimeSet_SO opponentNetworkedUnitRuntimeSet)
        {
            return new AiTowerBattleActions(ownerNetworkingUnitBehaviour, ownerBattleBehaviour, ownerUnitView,
                opponentNetworkedUnitRuntimeSet, damageProjectileBehaviour);
        }
    }
}
