using System.Collections.Generic;
using Features.Unit;
using UnityEngine;

public class UnitModHud : MonoBehaviour
{
    public UnitBehaviour Unit { get; set; }

    private List<DropHandler> modSlots;

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<DropHandler>().Init(Unit, i);
        }
    }
}
