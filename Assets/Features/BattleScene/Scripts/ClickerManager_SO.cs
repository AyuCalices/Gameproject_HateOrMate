using System.Collections.Generic;
using System.Linq;
using Features.Unit.Scripts;
using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Services.BattleBehaviour;
using Features.Unit.Scripts.Behaviours.States;
using UnityEngine;
using UnityEngine.Serialization;

namespace Features.BattleScene.Scripts
{
    [CreateAssetMenu]
    public class ClickerManager_SO : ScriptableObject
    {
        [FormerlySerializedAs("allUnitsRuntimeSet")] [SerializeField] private UnitServiceProviderRuntimeSet_SO unitServiceProviderRuntimeSet;
        [SerializeField] private UnitClassData_SO towerClassData;

        public void OnClick()
        {
            List<UnitServiceProvider> towerBattleActions = unitServiceProviderRuntimeSet.GetUnitsByTag(TeamTagType.Own);

            foreach (UnitServiceProvider battleBehaviour in towerBattleActions)
            {
                if (!battleBehaviour.TeamTagTypes.Contains(TeamTagType.Own) || battleBehaviour.GetService<UnitBattleBehaviour>().CurrentState is not AttackState) continue;
                
                if (battleBehaviour.UnitClassData == towerClassData)
                {
                    battleBehaviour.GetService<UnitBattleBehaviour>().BattleClass.OnPerformAction();
                }
            }
        }
    }
}
