using System.Collections.Generic;
using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Battle;
using UnityEngine;

namespace Features.Unit.Scripts
{
    [CreateAssetMenu]
    public class ClickerManager_SO : ScriptableObject
    {
        [SerializeField] private NetworkedUnitRuntimeSet_SO localUnitRuntimeSet;
        [SerializeField] private UnitClassData_SO towerClassData;

        public void OnClick()
        {
            List<NetworkedBattleBehaviour> towerBattleActions = localUnitRuntimeSet.GetItems();

            foreach (NetworkedBattleBehaviour towerBattleAction in towerBattleActions)
            {
                if (towerBattleAction is not BattleBehaviour {CurrentState: AttackState} battleBehaviour) continue;
                if (battleBehaviour.UnitClassData == towerClassData)
                {
                    battleBehaviour.BattleClass.OnPerformAction();
                }
            }
        }
    }
}
