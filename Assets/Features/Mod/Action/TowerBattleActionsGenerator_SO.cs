using Features.Battle;
using Features.GlobalReferences;
using Features.Unit.Battle;
using Features.Unit.Battle.Actions;
using Features.Unit.Modding;
using Features.Unit.View;
using UnityEngine;

namespace Features.Mod.Action
{
    [CreateAssetMenu(fileName = "new TowerBattleActions", menuName = "Unit/Actions/TowerBattleActions")]
    public class TowerBattleActionsGenerator_SO : BattleActionGenerator_SO
    {
        [SerializeField] private DamageProjectileBehaviour damageProjectileBehaviour;
        
        protected override BattleActions InternalGenerate(NetworkedUnitBehaviour ownerNetworkingUnitBehaviour, BattleBehaviour ownerBattleBehaviour,
            UnitView ownerUnitView, NetworkedUnitRuntimeSet_SO opponentNetworkedUnitRuntimeSet)
        {
            return new TowerBattleActions(ownerNetworkingUnitBehaviour, ownerBattleBehaviour, ownerUnitView,
                opponentNetworkedUnitRuntimeSet, damageProjectileBehaviour, 10, 10);
        }
    }
}
