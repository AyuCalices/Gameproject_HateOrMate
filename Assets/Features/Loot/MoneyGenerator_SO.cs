using DataStructures.Variables;
using UnityEngine;

namespace Features.Loot
{
    [CreateAssetMenu]
    public class MoneyGenerator_SO : LootableGenerator_SO
    {
        [SerializeField] private IntVariable localMoney;
        [SerializeField] private int amount;
    
        public override void OnAddInstanceToPlayer()
        {
            localMoney.Add(amount);
        }
    }
}
