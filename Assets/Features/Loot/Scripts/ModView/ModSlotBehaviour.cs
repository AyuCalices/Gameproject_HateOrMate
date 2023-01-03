using Features.Unit.Scripts.Behaviours.Stat;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Features.Loot.Scripts.ModView
{
    public class ModSlotBehaviour : MonoBehaviour, IDropHandler
    {
        [SerializeField] private DragControllerFocus_SO dragControllerFocus;
        [SerializeField] private Image image;
        [SerializeField] private Color freeColor;
        [SerializeField] private Color blockedColor;

        public ModDragBehaviour ContainedModDragBehaviour { get; set; }
        
        private bool _isActive;
        private NetworkedStatsBehaviour _localStats;

        public void Init(NetworkedStatsBehaviour localStats)
        {
            _localStats = localStats;
            _isActive = true;
        }

        public void UpdateSlot()
        {
            image.color = _isActive ? freeColor : blockedColor;
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null) return;
            eventData.pointerDrag.TryGetComponent(out ModDragBehaviour movingMod);
            if (movingMod == null) return;

            dragControllerFocus.Get().AddOrExchangeMod(ContainedModDragBehaviour, this);
        }

        public void ApplyModToInstantiatedUnit(NetworkedStatsBehaviour instantiatedUnit)
        {
            if (!ContainsMod()) return;
            if (!_isActive) return;
            
            ContainedModDragBehaviour.BaseMod.ApplyToInstantiatedUnit(instantiatedUnit);
        }

        private bool ContainsMod()
        {
            return ContainedModDragBehaviour != null;
        }

        //TODO: not clear enough what is target and what origin - use abstraction to differentiate between hand and slot
        public void AddOrExchangeMod(ModDragBehaviour newMod, ModSlotBehaviour origin)
        {
            if (this == origin) return;
            
            ModDragBehaviour removedMod = ContainedModDragBehaviour;
            
            //can be null due to hand
            if (ContainsMod())
            {
                RemoveMod(true);
            }

            //can be null due to hand
            if (origin != null)
            {
                //can be null due to hand
                if (origin.ContainsMod())
                {
                    origin.RemoveMod(true);
                }

                //can be null due to hand
                if (removedMod != null)
                {
                    origin.SwapAddMod(removedMod, origin);
                }
            }

            SwapAddMod(newMod, this);
        }

        private void SwapAddMod(ModDragBehaviour newModDragBehaviour, ModSlotBehaviour target)
        {
            if (target == null) return;
            
            if (!target._isActive)
            {
                newModDragBehaviour.BaseMod.DisableMod(target._localStats, false);
            }
            else if (target._isActive)
            {
                target.AddMod(newModDragBehaviour);
            }
        }

        private void AddMod(ModDragBehaviour newMod)
        {
            ContainedModDragBehaviour = newMod;

            if (_isActive) newMod.BaseMod.EnableMod(_localStats);
        }
        
        public void RemoveMod(bool isSwap)
        {
            if (_isActive && ContainsMod()) ContainedModDragBehaviour.BaseMod.DisableMod(_localStats, isSwap);
            
            ContainedModDragBehaviour = null;
        }

        public void UpdateActiveStatus()
        {
            if (_isActive)
            {
                ContainedModDragBehaviour.BaseMod.EnableMod(_localStats);
            }
            else
            {
                ContainedModDragBehaviour.BaseMod.DisableMod(_localStats, false);
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
                ContainedModDragBehaviour.BaseMod.DisableMod(_localStats, false);
            }
        }
        
        private void EnableSlot()
        {
            _isActive = true;
            
            if (ContainsMod())
            {
                ContainedModDragBehaviour.BaseMod.EnableMod(_localStats);
            }
        }
    }
}
