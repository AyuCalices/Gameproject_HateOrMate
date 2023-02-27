using System;
using Features.Battle.Scripts.Unit.ServiceLocatorSystem;
using Features.Connection.Scripts;
using Features.Loot.Scripts.ModView;
using Features.Unit.Scripts.Behaviours;
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

        public void EnableMod(UnitServiceProvider modifiedUnitServiceProvider, int slot)
        {
            if (_isEnabled) { return; }
            
            InternalAddMod(modifiedUnitServiceProvider, slot);
            _isEnabled = true;
        }
        
        public void DisableMod(UnitServiceProvider moddedLocalStats)
        {
            if (!_isEnabled) return;

            InternalRemoveMod(moddedLocalStats);
            _isEnabled = false;
        }

        public virtual bool IsValidAddMod(UnitServiceProvider requestedUnitServiceProvider, int slot, ErrorPopup errorPopup, Transform transform) { return true; }
        public virtual void ApplyToInstantiatedUnit(UnitServiceProvider instantiatedUnitServiceProvider) {}
        public virtual void ApplyOnUnitViewInstantiated(UnitViewBehaviour instantiatedView) {}
        protected abstract void InternalAddMod(UnitServiceProvider modifiedUnitServiceProvider, int slot);
        protected abstract void InternalRemoveMod(UnitServiceProvider modifiedUnitServiceProvider);
    }
}
