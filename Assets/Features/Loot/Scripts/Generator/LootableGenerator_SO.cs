using Features.Connection;
using Features.Connection.Scripts.Utils;
using UnityEngine;

namespace Features.Loot.Scripts.Generator
{
    public abstract class LootableGenerator_SO : NetworkedScriptableObject
    {
        [field: SerializeField] private string lootableName;
        [field: SerializeField] private string description;

        public string LootableName => lootableName;
        public string Description => description;

        public abstract void OnAddInstanceToPlayer();
    }
}
