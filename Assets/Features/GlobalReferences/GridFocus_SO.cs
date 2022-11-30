using DataStructures.Focus;
using UnityEngine;

namespace Features.GlobalReferences
{
    [CreateAssetMenu]
    public class GridFocus_SO : Focus_SO<GameObject>
    {
        public bool isFixedToMousePosition { get; set; }
    }
}
