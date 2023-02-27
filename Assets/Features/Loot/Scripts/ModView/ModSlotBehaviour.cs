using Features.Battle.Scripts.Unit.ServiceLocatorSystem;
using Features.Connection.Scripts;
using Features.Unit.Scripts.Behaviours;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Features.Loot.Scripts.ModView
{
    public class ModSlotBehaviour : MonoBehaviour, IDropHandler, IModContainer
    {
        [SerializeField] private Image image;
        [SerializeField] private Color freeColor;
        [SerializeField] private Color blockedColor;
        [SerializeField] private ErrorPopup errorPopup;

        public Transform Transform => transform;
        public bool ContainsMod => ContainedModBehaviour != null;
        public ModBehaviour ContainedModBehaviour { get; private set; }

        private bool _isActive;
        public bool IsActive => _isActive;
        private UnitServiceProvider _ownerUnitServiceProvider;
        private int _slot;
        
        public void Initialize(UnitServiceProvider localStats, int slot)
        {
            _ownerUnitServiceProvider = localStats;
            _isActive = true;
            _slot = slot;
        }
        
        public void ApplyToInstantiatedUnit(UnitServiceProvider instantiatedUnitServiceProvider)
        {
            if (!ContainsMod) return;
            if (!_isActive) return;
            
            ContainedModBehaviour.ContainedMod.ApplyToInstantiatedUnit(instantiatedUnitServiceProvider);
        }
        
        public void ApplyOnUnitViewInstantiated(UnitViewBehaviour instantiatedView)
        {
            if (!ContainsMod) return;
            if (!_isActive) return;
            
            ContainedModBehaviour.ContainedMod.ApplyOnUnitViewInstantiated(instantiatedView);
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null) return;
            eventData.pointerDrag.TryGetComponent(out ModBehaviour movingMod);
            if (movingMod == null) return;

            if (!movingMod.ContainedMod.IsValidAddMod(_ownerUnitServiceProvider, _slot, errorPopup, transform.root)) return;
            
            ModHelper.AddOrExchangeMod(movingMod, ContainsMod ? ContainedModBehaviour : null,
                movingMod.CurrentModContainer, this);
        }

        public void AddMod(ModBehaviour newModBehaviour)
        {
            ContainedModBehaviour = newModBehaviour;
            UpdateModColor();
            if (_isActive) newModBehaviour.ContainedMod.EnableMod(_ownerUnitServiceProvider, _slot);
        }
        
        public void RemoveMod(ModBehaviour removedModBehaviour)
        {
            if (_isActive) ContainedModBehaviour.ContainedMod.DisableMod(_ownerUnitServiceProvider);
            UpdateModColor();
            ContainedModBehaviour = null;
        }

        public void DisableSlot()
        {
            if (!_isActive) return;
            _isActive = false;
            UpdateSlotColor();

            if (!ContainsMod) return;
            ContainedModBehaviour.ContainedMod.DisableMod(_ownerUnitServiceProvider);
            UpdateModColor();
        }
        
        public void EnableSlot()
        {
            if (_isActive) return;
            _isActive = true;
            UpdateSlotColor();

            if (!ContainsMod) return;
            ContainedModBehaviour.ContainedMod.EnableMod(_ownerUnitServiceProvider, _slot);
            UpdateModColor();
        }
        
        private void UpdateSlotColor()
        {
            image.color = _isActive ? freeColor : blockedColor;
        }

        private void UpdateModColor()
        {
            if (ContainsMod)
            {
                ContainedModBehaviour.UpdateColor(_isActive ? freeColor : blockedColor);
            }
        }
    }
}
