using System.Collections.Generic;
using Features.GlobalReferences.Scripts;
using Features.Unit.Battle.Scripts.Actions;
using Features.Unit.Modding;
using UnityEngine;

namespace Features.Unit.Battle.Scripts
{
    [CreateAssetMenu]
    public class ClickerManager_SO : ScriptableObject
    {
        [SerializeField] private NetworkedUnitRuntimeSet_SO localUnitRuntimeSet;

        public void OnClick()
        {
            List<NetworkedUnitBehaviour> towerBattleActions = localUnitRuntimeSet.GetItems();

            foreach (NetworkedUnitBehaviour towerBattleAction in towerBattleActions)
            {
                if (towerBattleAction.TryGetComponent(out BattleBehaviour battleBehaviour) && battleBehaviour.CurrentState is AttackState)
                {
                    battleBehaviour.BattleActions.OnPerformAction();
                }
            }
        }
    }
}
