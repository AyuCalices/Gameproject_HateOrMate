using System;
using System.Collections.Generic;
using System.Linq;
using DataStructures.ReactiveVariable;
using Features.Unit.Scripts;
using UnityEngine;

namespace Features.BattleScene.Scripts.StageProgression
{
    [CreateAssetMenu(fileName = "StageRandomizer", menuName = "StageProgression/StageRandomizer")]
    public class StageRandomizer_SO : ScriptableObject
    {
        public static Action<string, UnitClassData_SO, int> onNetworkedSpawnUnit;
        
        [SerializeField] private IntReactiveVariable currentStage;
        
        public List<StageRange> stageRangeList;

        public void GenerateUnits()
        {
            int counter = 0;
            foreach (StageRange stageRange in stageRangeList)
            {
                counter += stageRange.stageCount;
                
                if (counter < currentStage.Get()) continue;
                
                SpawnUnits(stageRange);
                return;
            }
        }

        private void SpawnUnits(StageRange stageRange)
        {
            List<UnitDefinition> generatedUnits = stageRange.stageRandomizeData.GetUnitPool();

            foreach (UnitDefinition generatedUnit in generatedUnits)
            {
                onNetworkedSpawnUnit?.Invoke(generatedUnit.spawnerReference, generatedUnit.unitClassData, currentStage.Get());
            }
        }
    }

    [Serializable]
    public class StageRange
    {
        public int stageCount;
        public StageRandomizeData_SO stageRandomizeData;
    }
}
