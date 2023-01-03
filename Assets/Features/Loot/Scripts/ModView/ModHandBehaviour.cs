using System;
using System.Collections;
using System.Collections.Generic;
using Features.Loot.Scripts.GeneratedLoot;
using Features.Loot.Scripts.ModView;
using Features.Unit.Scripts.Behaviours.Mod;
using Features.Unit.Scripts.Behaviours.Stat;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ModHandBehaviour : MonoBehaviour, IDropHandler, IModContainer
{
    [SerializeField] private DragControllerFocus_SO dragControllerFocus;
    [SerializeField] private Transform dragTransform;
    [SerializeField] private Transform contentTransform;

    public List<ModBehaviour> ContainedModBehaviours { get; set; }
    public bool ContainsMod() => ContainedModBehaviours is {Count: > 0};
    public Transform Transform => contentTransform;
    
    public bool DisableModOnSwap() => true;

    public void SwapAddMod(ModBehaviour newModBehaviour)
    {
        ContainedModBehaviours.Add(newModBehaviour);
    }

    public void RemoveMod(ModBehaviour removedModBehaviour, bool isSwap)
    {
        ContainedModBehaviours.Remove(removedModBehaviour);
    }

    private void Awake()
    {
        BaseMod.onModInstantiated += InstantiateModToHand;
        UnitMods.onMoveToHand += MoveToHand;
        ContainedModBehaviours = new List<ModBehaviour>();
    }

    private void OnDestroy()
    {
        BaseMod.onModInstantiated -= InstantiateModToHand;
        UnitMods.onMoveToHand -= MoveToHand;
    }


    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;
        eventData.pointerDrag.TryGetComponent(out ModBehaviour movingMod);
        if (movingMod == null) return;
        
        dragControllerFocus.Get().AddOrExchangeMod(movingMod, null, movingMod.CurrentModSlotBehaviour, this);
        
        movingMod.GetComponent<ExpandBehaviour>().SetExpanded(true);
        movingMod.GetComponent<ExpandBehaviour>().IsActive = false;
        movingMod.isInHand = true;
    }

    private void MoveToHand(ModBehaviour modBehaviourPrefab)
    {
        SwapAddMod(modBehaviourPrefab);
        modBehaviourPrefab.SetNewOrigin(this);
    }
    
    private void InstantiateModToHand(ModBehaviour modBehaviourPrefab, BaseMod baseMod)
    {
        ModBehaviour modBehaviour = Instantiate(modBehaviourPrefab, contentTransform);
        modBehaviour.Initialize(baseMod, dragTransform, this);
        ContainedModBehaviours.Add(modBehaviourPrefab);
    }
}
