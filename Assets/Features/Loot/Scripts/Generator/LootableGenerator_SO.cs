using Features.Connection;
using Features.Connection.Scripts.Utils;
using UnityEngine;

namespace Features.Loot.Scripts.Generator
{
    public abstract class LootableGenerator_SO : NetworkedScriptableObject
    {
        [field: SerializeField] private GameObject spritePrefab;
        [field: SerializeField] private string description;

        public GameObject SpritePrefab => spritePrefab;
        public string Description => description;

        public abstract void OnAddInstanceToPlayer();
    }
}
