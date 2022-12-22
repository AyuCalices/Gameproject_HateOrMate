using System.Collections.Generic;
using Features.GlobalReferences.Scripts;
using UnityEngine;

namespace Features.Unit.Battle.Scripts
{
    [CreateAssetMenu]
    public class ClickerManager_SO : ScriptableObject
    {
        [SerializeField] private NetworkedUnitRuntimeSet_SO localUnitRuntimeSet;

        public void OnClick()
        {
            List<NetworkedBattleBehaviour> towerBattleActions = localUnitRuntimeSet.GetItems();

            foreach (NetworkedBattleBehaviour towerBattleAction in towerBattleActions)
            {
                if (towerBattleAction is BattleBehaviour {CurrentState: AttackState} battleBehaviour)
                {
                    battleBehaviour.BattleActions.OnPerformAction();
                }
            }
        }
    }
}
