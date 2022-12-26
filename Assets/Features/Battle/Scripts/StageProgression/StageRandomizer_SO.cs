using System;
using System.Collections.Generic;
using System.Linq;
using DataStructures.ReactiveVariable;
using Features.Unit.Scripts;
using Features.Unit.Scripts.Behaviours.Stat;
using UnityEngine;

namespace Features.Battle.Scripts.StageProgression
{
    [CreateAssetMenu(fileName = "StageRandomizer", menuName = "StageProgression/StageRandomizer")]
    public class StageRandomizer_SO : ScriptableObject
    {
        public static Action<string, UnitClassData_SO, BaseStats> onNetworkedSpawnUnit;
        
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
            List<UnitDefinition> generatedUnits = stageRange.GetUnitPool();

            foreach (UnitDefinition generatedUnit in generatedUnits)
            {
                BaseStats baseStats =
                    generatedUnit.synchronizedBaseStatsGenerator.GetSynchronizedStats();
                onNetworkedSpawnUnit?.Invoke(generatedUnit.spawnerReference, generatedUnit.unitClassData, baseStats);
            }
        }
    }

    [Serializable]
    public class StageRange
    {
        public int stageCount;
        public List<StageRandomizeData> stageRandomizeData;
        
        public List<UnitDefinition> GetUnitPool()
        {
            int count = stageRandomizeData.Sum(stageRandomizeData => stageRandomizeData.frequency);

            int selection = UnityEngine.Random.Range(0, count);

            int selectionCount = 0;
            foreach (StageRandomizeData stageRandomizeData in stageRandomizeData)
            {
                selectionCount += stageRandomizeData.frequency;
                if (selectionCount >= selection)
                {
                    return stageRandomizeData.stageRandomizeData.GetUnits();
                }
            }

            return stageRandomizeData[0].stageRandomizeData.unitPool;
        }
    }

    [Serializable]
    public class StageRandomizeData
    {
        public StageRandomizeData_SO stageRandomizeData;
        public int frequency;
    }
}
