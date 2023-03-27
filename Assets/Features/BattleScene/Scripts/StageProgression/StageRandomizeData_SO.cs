using System;
using System.Collections.Generic;
using Features.Unit.Scripts;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Features.BattleScene.Scripts.StageProgression
{
    [CreateAssetMenu(fileName = "StageRandomizeData", menuName = "StageProgression/StageRandomizeData")]
    public class StageRandomizeData_SO : ScriptableObject
    {
        [SerializeField] private List<UnitDefinition> staticUnits;
        [SerializeField] private int randomizedUnitsCount;
        [FormerlySerializedAs("unitPool")] public List<UnitDefinition> randomUnitsPool;

        public List<UnitDefinition> GetUnitPool()
        {
            List<UnitDefinition> generatedUnits = new List<UnitDefinition>();
            
            generatedUnits.AddRange(staticUnits);

            int randomizedUnitsPoolCount = randomUnitsPool.Count;
            for (int i = 0; i < randomizedUnitsCount; i++)
            {
                generatedUnits.Add(randomUnitsPool[Random.Range(0, randomizedUnitsPoolCount)]);
            }

            return generatedUnits;
        }
    }

    [Serializable]
    public class UnitDefinition
    {
        [Header("Unit Data")]
        public string spawnerReference;
        public UnitClassData_SO unitClassData;
    }
}
