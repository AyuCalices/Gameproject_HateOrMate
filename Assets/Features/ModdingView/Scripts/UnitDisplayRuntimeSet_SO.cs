using System;
using DataStructures.RuntimeSet;
using Features.Loot.Scripts;
using UnityEngine;

namespace Features.Unit.Scripts.Behaviours.Mod
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
