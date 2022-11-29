using System.Collections;
using System.Collections.Generic;
using DataStructures.Focus;
using UnityEngine;

[CreateAssetMenu]
public class GameObjectFocus_SO : Focus_SO<GameObject>
{
    public bool isFixedToMousePosition { get; set; }
}
