using Features.Unit.Scripts.Behaviours.Stat;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Features.Loot.Scripts.ModView
{
    public class ModSlotBehaviour : MonoBehaviour, IDropHandler, IModContainer
    {
        [SerializeField] private DragControllerFocus_SO dragControllerFocus;
        [SerializeField] private Image image;
        [SerializeField] private Color freeColor;
        [SerializeField] private Color blockedColor;

        public ModBehaviour ContainedModBehaviour { get; set; }
        public Transform Transform => transform;
        
        public bool ContainsMod() => ContainedModBehaviour != null;
        public bool DisableModOnSwap() => false;

        private bool _isActive;
        private NetworkedStatsBehaviour _localStats;

        public void Init(NetworkedStatsBehaviour localStats)
        {
            _localStats = localStats;
            _isActive = true;
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null) return;
            eventData.pointerDrag.TryGetComponent(out ModBehaviour movingMod);
            if (movingMod == null) return;

            dragControllerFocus.Get().AddOrExchangeMod(movingMod, ContainsMod() ? ContainedModBehaviour : null,
                movingMod.CurrentModSlotBehaviour, this);
            
            movingMod.isInHand = false;
        }

        public void ApplyModToInstantiatedUnit(NetworkedStatsBehaviour instantiatedUnit)
        {
            if (!ContainsMod()) return;
            if (!_isActive) return;
            
            ContainedModBehaviour.BaseMod.ApplyToInstantiatedUnit(instantiatedUnit);
        }

        public void SwapAddMod(ModBehaviour newModBehaviour)
        {
            if (!_isActive)
            {
                ContainedModBehaviour = newModBehaviour;
                newModBehaviour.BaseMod.DisableMod(_localStats, false);
            }
            else if (_isActive)
            {
                AddMod(newModBehaviour);
            }
            
            UpdateModColor();
        }

        private void AddMod(ModBehaviour newMod)
        {
            ContainedModBehaviour = newMod;
            
            if (_isActive) newMod.BaseMod.EnableMod(_localStats);
            
            UpdateModColor();
        }
        
        public void RemoveMod(ModBehaviour removedModBehaviour, bool isSwap)
        {
            if (_isActive && ContainsMod()) ContainedModBehaviour.BaseMod.DisableMod(_localStats, isSwap);
            
            ContainedModBehaviour = null;

            if (!isSwap)
            {
                UpdateModColor();
            }
        }

        public void UpdateActiveStatus()
        {
            if (_isActive)
            {
                ContainedModBehaviour.BaseMod.EnableMod(_localStats);
            }
            else
            {
                ContainedModBehaviour.BaseMod.DisableMod(_localStats, false);
            }
        }
        
        public void ToggleSlot()
        {
            if (_isActive)
            {
                DisableSlot();
            }
            else
            {
                EnableSlot();
            }
        }

        public void DisableSlot()
        {
            _isActive = false;

            if (ContainsMod())
            {
                ContainedModBehaviour.BaseMod.DisableMod(_localStats, false);
            }

            UpdateSlot();
            UpdateModColor();
        }
        
        private void EnableSlot()
        {
            _isActive = true;
            
            if (ContainsMod())
            {
                ContainedModBehaviour.BaseMod.EnableMod(_localStats);
            }

            UpdateSlot();
            UpdateModColor();
        }
        
        private void UpdateSlot()
        {
            image.color = _isActive ? freeColor : blockedColor;
        }

        private void UpdateModColor()
        {
            if (ContainsMod())
            {
                ContainedModBehaviour.UpdateColor(_isActive ? freeColor : blockedColor);
            }
        }
    }
}
