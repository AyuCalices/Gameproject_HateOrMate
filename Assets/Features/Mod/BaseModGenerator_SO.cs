using Features.Unit;
using UnityEngine;

namespace Features.Mod
{
    public abstract class BaseModGenerator_SO : ScriptableObject
    {
        //TODO: put this into a class not derived from scriptable object -> scriptable object for instantiating
        //TODO: [field: SerializeField] private ModBehaviour modPrefab;
        [field: SerializeField] private string modName;
        [field: SerializeField] private string description;

        public string ModName => modName;
        public string Description => description;

        public abstract BaseMod Generate();
    }
}
