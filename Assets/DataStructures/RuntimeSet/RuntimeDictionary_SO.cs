using System.Collections.Generic;
using UnityEngine;

namespace DataStructures.RuntimeSet
{
    public class RuntimeDictionary_SO<Key, Value> : ScriptableObject
    {
        [SerializeField] protected readonly Dictionary<Key, Value> items = new Dictionary<Key, Value>();

        public Dictionary<Key, Value> GetItems()
        {
            return items;
        }

        public bool TryGetValue(Key key, out Value value)
        {
            return items.TryGetValue(key, out value);
        }

        public void Add(Key key, Value value)
        {
            if (!items.ContainsKey(key))
            {
                items.Add(key, value);
            }
        }

        public void Remove(Key key)
        {
            if (items.ContainsKey(key))
            {
                items.Remove(key);
            }
        }

        public void Restore()
        {
            items.Clear();
        }
    }
}
