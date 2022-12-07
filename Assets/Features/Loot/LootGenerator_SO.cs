using System;
using System.Collections.Generic;
using System.Linq;
using Features.Mod;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Features.Loot
{
    [CreateAssetMenu]
    public class LootGenerator_SO : ScriptableObject
    {
        [SerializeField] private List<LootFrequency> baseModGenerators;

        public BaseMod GenerateNew()
        {
            int sum = baseModGenerators.Sum(x => x.frequency);
            int selectedLoot = Random.Range(0, sum);

            int acc = 0;
            foreach (LootFrequency baseModGenerator in baseModGenerators)
            {
                acc += baseModGenerator.frequency;
                if (acc >= selectedLoot)
                {
                    return baseModGenerator.modGenerator.Generate();
                }
            }

            return null;
        }

        public List<BaseMod> GetFirstGeneratedElements(List<BaseMod> generatedMods, int requestedCount)
        {
            List<BaseMod> result = generatedMods.GetRange(0, Mathf.Min(generatedMods.Count, requestedCount));
            generatedMods.RemoveRange(0, requestedCount);
            return result;
        }
    }

    [Serializable]
    public struct LootFrequency
    {
        public BaseModGenerator_SO modGenerator;
        public int frequency;
    }
}