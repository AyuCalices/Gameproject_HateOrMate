using System;
using Features.Connection.Scripts;
using Features.Loot.Scripts.ModView;
using Features.Unit.Scripts.Behaviours.Stat;
using UnityEngine;

namespace Features.Loot.Scripts.GeneratedLoot
{
    public abstract class BaseMod
    {
        public static Action<ModBehaviour, BaseMod> onModInstantiated;
        
        public GameObject SpritePrefab { get; }
        public string Description { get; }
        public int Level { get; }
        
        //make sure a mod can't be added twice
        private bool _isEnabled;
        private readonly ModBehaviour _modBehaviourPrefab;

        protected BaseMod(GameObject spritePrefab, string description, int level, ModBehaviour modBehaviourPrefab)
        {
            _modBehaviourPrefab = modBehaviourPrefab;
            SpritePrefab = spritePrefab;
            Description = description;
            _isEnabled = false;
            Level = level;
        }

        public void RaiseOnModInstantiated()
        {
            onModInstantiated?.Invoke(_modBehaviourPrefab, this);
        }

        public void EnableMod(NetworkedStatsBehaviour moddedLocalStats, int slot)
        {
            if (_isEnabled) { return; }
            
            InternalAddMod(moddedLocalStats, slot);
            _isEnabled = true;
        }
        
        public void DisableMod(NetworkedStatsBehaviour moddedLocalStats)
        {
            if (!_isEnabled) return;

            InternalRemoveMod(moddedLocalStats);
            _isEnabled = false;
        }

        public virtual bool IsValidAddMod(NetworkedStatsBehaviour instantiatedUnit, int slot, ErrorPopup errorPopup, Transform transform) { return true; }
        public virtual void ApplyToInstantiatedUnit(NetworkedStatsBehaviour instantiatedUnit) {}
        protected abstract void InternalAddMod(NetworkedStatsBehaviour moddedLocalStats, int slot);
        protected abstract void InternalRemoveMod(NetworkedStatsBehaviour moddedLocalStats);
    }
}
