using System.Collections.Generic;
using Features.Mods.Scripts.ModTypes;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Features.Mods.Scripts.View.ModContainer
{
    public class HandModContainerBehaviour : MonoBehaviour, IDropHandler, IModContainer
    {
        [SerializeField] private Transform dragTransform;
        [SerializeField] private Transform contentTransform;

        public Transform Transform => contentTransform;
        public bool ContainsMod => ContainedModBehaviours is {Count: > 0};
        private List<ModViewBehaviour> ContainedModBehaviours { get; set; }

        private void Awake()
        {
            BaseMod.onModInstantiated += InstantiateModToHand;
            UnitDisplayMods.onDestroyUnit += MoveToHandOnDestroyUnit;
            ContainedModBehaviours = new List<ModViewBehaviour>();
        }

        private void OnDestroy()
        {
            BaseMod.onModInstantiated -= InstantiateModToHand;
            UnitDisplayMods.onDestroyUnit -= MoveToHandOnDestroyUnit;
        }

        private void InstantiateModToHand(ModViewBehaviour modViewBehaviourPrefab, BaseMod baseMod)
        {
            ModViewBehaviour modViewBehaviour = Instantiate(modViewBehaviourPrefab, contentTransform);
            modViewBehaviour.Initialize(baseMod, dragTransform, this);

            RegisterModOnHand(modViewBehaviour);
        }
        
        private void RegisterModOnHand(ModViewBehaviour newModViewBehaviour)
        {
            ContainedModBehaviours.Add(newModViewBehaviour);
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null) return;
            eventData.pointerDrag.TryGetComponent(out ModViewBehaviour movingMod);
            if (movingMod == null) return;
        
            ModSwapHelper.Perform(movingMod, null, movingMod.CurrentModContainer, this);
        }

        //Called by ModHelper
        public void AddMod(ModViewBehaviour newModViewBehaviour)
        {
            RegisterModOnHand(newModViewBehaviour);
            newModViewBehaviour.UpdateColor(Color.white);
        }

        //Called by ModHelper
        public void RemoveMod(ModViewBehaviour removedModViewBehaviour)
        {
            ContainedModBehaviours.Remove(removedModViewBehaviour);
        }

        private void MoveToHandOnDestroyUnit(ModViewBehaviour newModViewBehaviour)
        {
            AddMod(newModViewBehaviour);
            newModViewBehaviour.InitializeNewOrigin(this);
        }
    }
}
