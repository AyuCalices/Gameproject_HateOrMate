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
        [SerializeField] private Transform modParent;
        [SerializeField] private Canvas canvas;
        [SerializeField] private int unFocusedValue;
        [SerializeField] private int focusedValue;

        public Transform Transform => modParent;
        public bool ContainsMod => ContainedModBehaviour != null;
        public ModBehaviour ContainedModBehaviour { get; private set; }

        private bool _isActive;
        public bool IsActive => _isActive;
        private NetworkedStatsBehaviour _localStats;
        private int _slot;
        
        public void Initialize(NetworkedStatsBehaviour localStats, int slot)
        {
            _localStats = localStats;
            _isActive = true;
            _slot = slot;
            Highlight(false);
        }

        public void Highlight(bool focused)
        {
            canvas.sortingOrder = focused ? focusedValue : unFocusedValue;
        }
        
        public void ApplyModToNewInstantiatedUnit(NetworkedStatsBehaviour instantiatedUnit)
        {
            if (!ContainsMod) return;
            if (!_isActive) return;
            
            ContainedModBehaviour.ContainedMod.ApplyToInstantiatedUnit(instantiatedUnit);
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null) return;
            eventData.pointerDrag.TryGetComponent(out ModBehaviour movingMod);
            if (movingMod == null) return;

            if (!movingMod.ContainedMod.IsValidAddMod(_localStats, _slot, errorPopup, transform.root)) return;
            
            ModHelper.AddOrExchangeMod(movingMod, ContainsMod ? ContainedModBehaviour : null,
                movingMod.CurrentModContainer, this);
        }

        public void AddMod(ModBehaviour newModBehaviour)
        {
            ContainedModBehaviour = newModBehaviour;
            UpdateModColor();
            if (_isActive) newModBehaviour.ContainedMod.EnableMod(_localStats, _slot);
        }
        
        public void RemoveMod(ModBehaviour removedModBehaviour)
        {
            if (_isActive) ContainedModBehaviour.ContainedMod.DisableMod(_localStats);
            UpdateModColor();
            ContainedModBehaviour = null;
        }

        public void DisableSlot()
        {
            if (!_isActive) return;
            _isActive = false;
            UpdateSlotColor();

            if (!ContainsMod) return;
            ContainedModBehaviour.ContainedMod.DisableMod(_localStats);
            UpdateModColor();
        }
        
        public void EnableSlot()
        {
            if (_isActive) return;
            _isActive = true;
            UpdateSlotColor();

            if (!ContainsMod) return;
            ContainedModBehaviour.ContainedMod.EnableMod(_localStats, _slot);
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
