using Features.General.UI.Scripts;
using Features.Unit.Scripts.Behaviours;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Features.Mods.Scripts.View.ModContainer
{
    public class UnitModContainerBehaviour : MonoBehaviour, IDropHandler, IModContainer
    {
        [SerializeField] private Image image;
        [SerializeField] private Color freeColor;
        [SerializeField] private Color blockedColor;
        [SerializeField] private ErrorPopup errorPopup;

        public Transform Transform => transform;
        public bool ContainsMod => ContainedModViewBehaviour != null;
        public ModViewBehaviour ContainedModViewBehaviour { get; private set; }

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
            
            ContainedModViewBehaviour.ContainedMod.ApplyToInstantiatedUnit(instantiatedUnitServiceProvider);
        }
        
        public void ApplyOnUnitViewInstantiated(UnitDisplayBehaviour unitDisplayBehaviour)
        {
            if (!ContainsMod) return;
            if (!_isActive) return;
            
            ContainedModViewBehaviour.ContainedMod.ApplyOnUnitViewInstantiated(unitDisplayBehaviour);
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null) return;
            eventData.pointerDrag.TryGetComponent(out ModViewBehaviour movingMod);
            if (movingMod == null) return;

            if (!movingMod.ContainedMod.IsValidAddMod(_ownerUnitServiceProvider, _slot, errorPopup, transform.root)) return;
            
            ModSwapHelper.Perform(movingMod, ContainsMod ? ContainedModViewBehaviour : null,
                movingMod.CurrentModContainer, this);
        }

        public void AddMod(ModViewBehaviour newModViewBehaviour)
        {
            ContainedModViewBehaviour = newModViewBehaviour;
            UpdateModColor();
            if (_isActive) newModViewBehaviour.ContainedMod.EnableMod(_ownerUnitServiceProvider, _slot);
        }
        
        public void RemoveMod(ModViewBehaviour removedModViewBehaviour)
        {
            if (_isActive) ContainedModViewBehaviour.ContainedMod.DisableMod(_ownerUnitServiceProvider);
            UpdateModColor();
            ContainedModViewBehaviour = null;
        }

        public void DisableSlot()
        {
            if (!_isActive) return;
            _isActive = false;
            UpdateSlotColor();

            if (!ContainsMod) return;
            ContainedModViewBehaviour.ContainedMod.DisableMod(_ownerUnitServiceProvider);
            UpdateModColor();
        }
        
        public void EnableSlot()
        {
            if (_isActive) return;
            _isActive = true;
            UpdateSlotColor();

            if (!ContainsMod) return;
            ContainedModViewBehaviour.ContainedMod.EnableMod(_ownerUnitServiceProvider, _slot);
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
                ContainedModViewBehaviour.UpdateColor(_isActive ? freeColor : blockedColor);
            }
        }
    }
}
