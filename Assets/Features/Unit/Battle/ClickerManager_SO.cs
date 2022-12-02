using System.Collections.Generic;
using Features.GlobalReferences;
using UnityEngine;

namespace Features.Unit.Battle
{
    [CreateAssetMenu]
    public class ClickerManager_SO : ScriptableObject
    {
        [SerializeField] private NetworkedUnitRuntimeSet_SO localUnitRuntimeSet;

        public void OnClick()
        {
            List<TowerBattleActions> towerBattleActions =
                localUnitRuntimeSet.GetAllUnitsByBattleAction<TowerBattleActions>();

            foreach (TowerBattleActions towerBattleAction in towerBattleActions)
            {
                towerBattleAction.OnPerformAction();
            }
        }
    }
}
