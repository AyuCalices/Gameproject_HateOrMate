using Features.Unit;
using UnityEngine;

namespace Features.Mod
{
    public abstract class BaseModGenerator_SO : ScriptableObject
    {
        [field: SerializeField] private string modName;
        [field: SerializeField] private string description;

        public string ModName => modName;
        public string Description => description;

        public abstract BaseMod Generate();
    }
}
