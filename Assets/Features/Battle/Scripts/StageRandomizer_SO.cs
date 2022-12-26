using System;
using System.Collections.Generic;
using System.Linq;
using DataStructures.ReactiveVariable;
using Features.Unit.Classes;
using UnityEngine;
using Random = System.Random;

namespace Features.Battle.Scripts
{
    [CreateAssetMenu(fileName = "StageRandomizer", menuName = "StageProgression/StageRandomizer")]
    public class StageRandomizer_SO : ScriptableObject
    {
        public static Action<string, UnitClassData_SO, SynchronizedBaseStats> onNetworkedSpawnUnit;
        
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

            Debug.Log(generatedUnits.Count);
            foreach (UnitDefinition generatedUnit in generatedUnits)
            {
                SynchronizedBaseStats synchronizedBaseStats =
                    generatedUnit.synchronizedBaseStatsGenerator.GetSynchronizedStats();
                onNetworkedSpawnUnit?.Invoke(generatedUnit.spawnerReference, generatedUnit.unitClassData, synchronizedBaseStats);
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
