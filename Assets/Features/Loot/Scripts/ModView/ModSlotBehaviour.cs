using Features.Connection.Scripts;
using Features.Unit.Scripts.Behaviours.Stat;
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
        public bool DisableModOnSwap => false;
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
            movingMod.IsSuccessfulDrop = true;
        }

        public void AddMod(ModBehaviour newModBehaviour)
        {
            ContainedModBehaviour = newModBehaviour;
            
            if (_isActive)
            {
                newModBehaviour.ContainedMod.EnableMod(_localStats, _slot);
            }
            else
            {
                newModBehaviour.ContainedMod.DisableMod(_localStats, false);
            }

            UpdateModColor();
        }
        
        public void RemoveMod(ModBehaviour removedModBehaviour, bool isSwap)
        {
            if (_isActive) ContainedModBehaviour.ContainedMod.DisableMod(_localStats, isSwap);
            
            ContainedModBehaviour = null;

            UpdateModColor();
        }

        public void DisableSlot()
        {
            if (!_isActive) return;
            
            _isActive = false;

            if (ContainsMod)
            {
                ContainedModBehaviour.ContainedMod.DisableMod(_localStats, false);
            }

            UpdateSlotColor();
            UpdateModColor();
        }
        
        public void EnableSlot()
        {
            if (_isActive) return;
            
            _isActive = true;
            
            if (ContainsMod)
            {
                ContainedModBehaviour.ContainedMod.EnableMod(_localStats, _slot);
            }

            UpdateSlotColor();
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
