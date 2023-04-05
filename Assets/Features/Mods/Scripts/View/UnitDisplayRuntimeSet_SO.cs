using DataStructures.RuntimeSet;
using UnityEngine;

namespace Features.Mods.Scripts.View
{
    [CreateAssetMenu]
    public class UnitDisplayRuntimeSet_SO : RuntimeSet_SO<UnitDisplayBehaviour>
    {
        private void OnEnable()
        {
            Restore();
        }
    }
}
