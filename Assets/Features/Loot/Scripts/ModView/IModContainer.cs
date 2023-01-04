using System.Collections.Generic;
using UnityEngine;

namespace Features.Loot.Scripts.ModView
{
    public interface IModContainer
    {
        public Transform Transform { get; }

        public bool ContainsMod { get; }

        public bool DisableModOnSwap { get; }

        public void AddMod(ModBehaviour newModBehaviour);
        
        public void RemoveMod(ModBehaviour removedModBehaviour, bool isSwap);
    }
}
