using Features.Battle;
using Features.Battle.Scripts;
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
    [CreateAssetMenu(fileName = "new TowerBattleActions", menuName = "Unit/Actions/TowerBattleActions")]
    public class TowerBattleActionsGenerator_SO : BattleActionGenerator_SO
    {
        [SerializeField] private DamageProjectileBehaviour damageProjectileBehaviour;
        
        protected override BattleActions InternalGenerate(NetworkedStatsBehaviour ownerNetworkingStatsBehaviour, BattleBehaviour ownerBattleBehaviour,
            UnitView ownerUnitView)
        {
            return new TowerBattleActions(ownerNetworkingStatsBehaviour, ownerBattleBehaviour, ownerUnitView, damageProjectileBehaviour, 10, 10);
        }
    }
}
