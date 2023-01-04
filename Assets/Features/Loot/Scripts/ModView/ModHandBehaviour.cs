using System.Collections.Generic;
using Features.Loot.Scripts.GeneratedLoot;
using Features.Unit.Scripts.Behaviours.Mod;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Features.Loot.Scripts.ModView
{
    public class ModHandBehaviour : MonoBehaviour, IDropHandler, IModContainer
    {
        [SerializeField] private Transform dragTransform;
        [SerializeField] private Transform contentTransform;
    
        public Transform Transform => contentTransform;
        public bool ContainsMod => ContainedModBehaviours is {Count: > 0};
        public bool DisableModOnSwap => true;
        private List<ModBehaviour> ContainedModBehaviours { get; set; }

        private void Awake()
        {
            BaseMod.onModInstantiated += InstantiateModToHand;
            UnitMods.onDestroyUnit += MoveToHandOnDestroyUnit;
            ContainedModBehaviours = new List<ModBehaviour>();
        }

        private void OnDestroy()
        {
            BaseMod.onModInstantiated -= InstantiateModToHand;
            UnitMods.onDestroyUnit -= MoveToHandOnDestroyUnit;
        }
        
        private void InstantiateModToHand(ModBehaviour modBehaviourPrefab, BaseMod baseMod)
        {
            ModBehaviour modBehaviour = Instantiate(modBehaviourPrefab, contentTransform);
            modBehaviour.Initialize(baseMod, dragTransform, this);
            RegisterModOnHand(modBehaviour);
        }
        
        private void RegisterModOnHand(ModBehaviour newModBehaviour)
        {
            newModBehaviour.IsInHand = true;
            ContainedModBehaviours.Add(newModBehaviour);
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null) return;
            eventData.pointerDrag.TryGetComponent(out ModBehaviour movingMod);
            if (movingMod == null) return;
        
            ModHelper.AddOrExchangeMod(movingMod, null, movingMod.CurrentModContainer, this);
            
            movingMod.IsSuccessfulDrop = true;
            SetModHandBehaviour(movingMod);
        }

        //Called by ModHelper
        public void AddMod(ModBehaviour newModBehaviour)
        {
            RegisterModOnHand(newModBehaviour);
            SetModHandBehaviour(newModBehaviour);
        }

        //Called by ModHelper
        public void RemoveMod(ModBehaviour removedModBehaviour, bool isSwap)
        {
            removedModBehaviour.IsInHand = false;
            ContainedModBehaviours.Remove(removedModBehaviour);
        }

        private void MoveToHandOnDestroyUnit(ModBehaviour newModBehaviour)
        {
            AddMod(newModBehaviour);
            SetModHandBehaviour(newModBehaviour);
            newModBehaviour.SetNewOrigin(this);
        }

        private void SetModHandBehaviour(ModBehaviour modBehaviour)
        {
            modBehaviour.ExpandBehaviour.SetExpanded(true);
            modBehaviour.ExpandBehaviour.IsActive = false;
            modBehaviour.UpdateColor(Color.white);
        }
    }
}
