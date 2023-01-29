using System;
using System.Collections.Generic;
using Features.Unit.Scripts;
using UnityEngine;

namespace Features.Battle.Scripts.StageProgression
{
    [CreateAssetMenu(fileName = "StageRandomizeData", menuName = "StageProgression/StageRandomizeData")]
    public class StageRandomizeData_SO : ScriptableObject
    {
        public List<UnitDefinition> unitPool;

        public List<UnitDefinition> GetUnits()
        {
            List<UnitDefinition> generatedUnits = new List<UnitDefinition>();
            
            foreach (UnitDefinition unit in unitPool)
            {
                for (int i = 0; i < unit.tries; i++)
                {
                    if (UnityEngine.Random.Range(0, 100) <= unit.spawnChance)
                    {
                        generatedUnits.Add(unit);
                    }
                }
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

        [Header("SpawnChance")][Range(0, 100)]
        public int spawnChance;
        public int tries;
    }
}
