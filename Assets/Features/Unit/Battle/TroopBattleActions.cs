using Features.GlobalReferences;
using Features.Unit.Modding;
using Features.Unit.Modding.Stat;
using UnityEngine;

namespace Features.Unit.Battle
{
    public class TroopBattleActions : BattleActions
    {
        private float _attackSpeedDeltaTime;
        
        public TroopBattleActions(NetworkedUnitBehaviour ownerNetworkingUnitBehaviour, NetworkedUnitRuntimeSet_SO opponentNetworkedUnitRuntimeSet, UnitView unitView) : base(ownerNetworkingUnitBehaviour, opponentNetworkedUnitRuntimeSet, unitView)
        {
            _attackSpeedDeltaTime = ownerNetworkingUnitBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Speed);
        }
        
        protected override void InternalUpdateBattleActions(NetworkedUnitBehaviour ownerNetworkingUnitBehaviour, UnitView unitView)
        {
            _attackSpeedDeltaTime -= Time.deltaTime;
            if (_attackSpeedDeltaTime <= 0)
            {
                _attackSpeedDeltaTime = ownerNetworkingUnitBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Speed);
                InternalOnPerformAction(ownerNetworkingUnitBehaviour, opponentNetworkedUnitRuntimeSet);
            }
        }

        protected override void InternalOnPerformAction(NetworkedUnitBehaviour ownerNetworkingUnitBehaviour,
            NetworkedUnitRuntimeSet_SO opponentNetworkedUnitRuntimeSet)
        {
            RaiseSendAttackEvent(
                opponentNetworkedUnitRuntimeSet.GetClosestByWorldPosition(ownerNetworkingUnitBehaviour.transform.position).Key.ViewID, 
                ownerNetworkingUnitBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Damage
                ));
        }

        public override void OnSendAttackActionCallback(BattleBehaviour targetBattleBehaviour, float value)
        {
            throw new System.NotImplementedException();
        }

        public override void OnSendHealthActionCallback(BattleBehaviour updateUnit, float newRemovedHealth, float totalHealth)
        {
            throw new System.NotImplementedException();
        }
    }
}
