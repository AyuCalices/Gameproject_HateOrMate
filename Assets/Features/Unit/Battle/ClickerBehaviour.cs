using System.Collections.Generic;
using Features.GlobalReferences;
using UnityEngine;

namespace Features.Unit.Battle
{
    public class ClickerBehaviour : MonoBehaviour
    {
        [SerializeField] private LocalUnitRuntimeSet_SO localUnitRuntimeSet;

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
