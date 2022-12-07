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

        public LootableGenerator_SO RandomizeLootableGenerator()
        {
            int sum = baseModGenerators.Sum(x => x.frequency);
            int selectedLoot = Random.Range(0, sum);

            int acc = 0;
            foreach (LootFrequency baseModGenerator in baseModGenerators)
            {
                acc += baseModGenerator.frequency;
                if (acc >= selectedLoot)
                {
                    return baseModGenerator.modGenerator;
                }
            }

            return null;
        }
    }

    [Serializable]
    public struct LootFrequency
    {
        public LootableGenerator_SO modGenerator;
        public int frequency;
    }
}