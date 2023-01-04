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

        public Transform Transform => transform;
        public bool ContainsMod => ContainedModBehaviour != null;
        public bool DisableModOnSwap => false;
        public ModBehaviour ContainedModBehaviour { get; private set; }

        private bool _isActive;
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

            if (movingMod.ContainedMod.IsValidAddMod(_localStats))
            {
                if (ContainsMod)
                {
                    if (movingMod.CurrentModContainer is ModSlotBehaviour modSlotBehaviour)
                    {
                        if (ContainedModBehaviour.ContainedMod.IsValidAddMod(modSlotBehaviour._localStats))
                        {
                            ModHelper.AddOrExchangeMod(movingMod, ContainedModBehaviour,
                                movingMod.CurrentModContainer, this);
                        }
                        else
                        {
                            Debug.LogWarning("cant add a unit to itself");
                        }
                    }
                    else
                    {
                        ModHelper.AddOrExchangeMod(movingMod, ContainedModBehaviour,
                            movingMod.CurrentModContainer, this);
                    }
                }
                else
                {
                    ModHelper.AddOrExchangeMod(movingMod, null,
                        movingMod.CurrentModContainer, this);
                }
            }
            else
            {
                Debug.LogWarning("cant add a unit to itself");
            }

            
            
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
