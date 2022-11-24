using System.Collections.Generic;
using Features.Unit;
using UnityEngine;

public class UnitModHud : MonoBehaviour
{
    public List<ModSlotBehaviour> GetAllChildren()
    {
        List<ModSlotBehaviour> modSlots = new List<ModSlotBehaviour>();
        
        for (int i = 0; i < transform.childCount; i++)
        {
            modSlots.Add(transform.GetChild(i).GetComponent<ModSlotBehaviour>());
        }

        return modSlots;
    }
}
