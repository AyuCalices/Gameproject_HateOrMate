using UnityEngine;

namespace Features.Mods.Scripts.View.ModContainer
{
    public interface IModContainer
    {
        public Transform Transform { get; }

        public bool ContainsMod { get; }

        public void AddMod(ModViewBehaviour newModViewBehaviour);
        
        public void RemoveMod(ModViewBehaviour removedModViewBehaviour);
    }
}
