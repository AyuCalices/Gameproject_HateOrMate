using DataStructures.Variables;
using UnityEngine;

namespace Features.Loot.Scripts.Generator
{
    [CreateAssetMenu]
    public class MoneyGenerator_SO : LootableGenerator_SO
    {
        [SerializeField] private IntVariable localMoney;
        [SerializeField] private int amount;
    
        public override void OnAddInstanceToPlayer(int stageAsLevel)
        {
            localMoney.Add(amount);
        }
    }
}
