using System;
using System.Collections.Generic;
using Features.Loot.Scripts.GeneratedLoot;
using Features.Unit.Scripts.Behaviours.Mod;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Features.Loot.Scripts.ModView
{
    public class HandModContainerBehaviour : MonoBehaviour, IDropHandler, IModContainer
    {
        [SerializeField] private Transform dragTransform;
        [SerializeField] private Transform contentTransform;

        public Transform Transform => contentTransform;
        public bool ContainsMod => ContainedModBehaviours is {Count: > 0};
        private List<ModBehaviour> ContainedModBehaviours { get; set; }

        private void Awake()
        {
            BaseMod.onModInstantiated += InstantiateModToHand;
            UnitDisplayMods.onDestroyUnit += MoveToHandOnDestroyUnit;
            ContainedModBehaviours = new List<ModBehaviour>();
        }

        private void OnDestroy()
        {
            BaseMod.onModInstantiated -= InstantiateModToHand;
            UnitDisplayMods.onDestroyUnit -= MoveToHandOnDestroyUnit;
        }

        private void InstantiateModToHand(ModBehaviour modBehaviourPrefab, BaseMod baseMod)
        {
            ModBehaviour modBehaviour = Instantiate(modBehaviourPrefab, contentTransform);
            modBehaviour.Initialize(baseMod, dragTransform, this);

            RegisterModOnHand(modBehaviour);
        }
        
        private void RegisterModOnHand(ModBehaviour newModBehaviour)
        {
            ContainedModBehaviours.Add(newModBehaviour);
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null) return;
            eventData.pointerDrag.TryGetComponent(out ModBehaviour movingMod);
            if (movingMod == null) return;
        
            ModSwapHelper.Perform(movingMod, null, movingMod.CurrentModContainer, this);
        }

        //Called by ModHelper
        public void AddMod(ModBehaviour newModBehaviour)
        {
            RegisterModOnHand(newModBehaviour);
            newModBehaviour.UpdateColor(Color.white);
        }

        //Called by ModHelper
        public void RemoveMod(ModBehaviour removedModBehaviour)
        {
            ContainedModBehaviours.Remove(removedModBehaviour);
        }

        private void MoveToHandOnDestroyUnit(ModBehaviour newModBehaviour)
        {
            AddMod(newModBehaviour);
            newModBehaviour.UpdateColor(Color.white);
            newModBehaviour.InitializeNewOrigin(this);
        }
    }
}
