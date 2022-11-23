using System;
using System.Collections;
using System.Collections.Generic;
using Features.ModHUD;
using Features.Unit;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropHandler : MonoBehaviour, IDropHandler
{
    private UnitBehaviour _unit;
    private int _slot;

    public void Init(UnitBehaviour unit, int slot)
    {
        _unit = unit;
        _slot = slot;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;
        eventData.pointerDrag.TryGetComponent(out DragHandler dragHandler);
        if (dragHandler == null) return;

        if (_unit.UnitMods.modSlots[_slot].ContainsMod())
        {
            _unit.UnitMods.modSlots[_slot].RemoveMod();
            //TODO: put the mod back to the origin
        }
        
        _unit.UnitMods.modSlots[_slot].AddMod(dragHandler.BaseMod);

        eventData.pointerDrag.transform.position = transform.position;
    }
}
