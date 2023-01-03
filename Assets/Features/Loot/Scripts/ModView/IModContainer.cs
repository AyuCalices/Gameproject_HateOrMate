using System.Collections.Generic;
using UnityEngine;

namespace Features.Loot.Scripts.ModView
{
    public interface IModContainer
    {
        public Transform Transform { get; }

        public bool ContainsMod();

        public bool DisableModOnSwap();

        public void SwapAddMod(ModBehaviour newModBehaviour);
        
        public void RemoveMod(ModBehaviour removedModBehaviour, bool isSwap);
    }
}
