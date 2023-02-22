using System;
using DataStructures.RuntimeSet;
using Features.Loot.Scripts;
using UnityEngine;

namespace Features.Unit.Scripts.Behaviours.Mod
{
    [CreateAssetMenu]
    public class UnitViewRuntimeSet_SO : RuntimeSet_SO<UnitViewBehaviour>
    {
        private void OnEnable()
        {
            Restore();
        }
    }
}
