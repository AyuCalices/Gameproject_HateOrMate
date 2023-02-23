using System.Collections.Generic;
using System.Linq;
using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Battle;
using UnityEngine;

namespace Features.Unit.Scripts
{
    [CreateAssetMenu]
    public class ClickerManager_SO : ScriptableObject
    {
        [SerializeField] private NetworkedUnitRuntimeSet_SO allUnitsRuntimeSet;
        [SerializeField] private UnitClassData_SO towerClassData;

        public void OnClick()
        {
            List<NetworkedBattleBehaviour> towerBattleActions = allUnitsRuntimeSet.GetUnitsByTag(TeamTagType.Own);

            foreach (NetworkedBattleBehaviour battleBehaviour in towerBattleActions)
            {
                if (!battleBehaviour.TeamTagTypes.Contains(TeamTagType.Own) || battleBehaviour.CurrentState is not AttackState) continue;
                
                if (battleBehaviour.UnitClassData == towerClassData)
                {
                    battleBehaviour.BattleClass.OnPerformAction();
                }
            }
        }
    }
}
