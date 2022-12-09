using UnityEngine;

namespace DataStructures.ReactiveVariable
{
    [CreateAssetMenu(fileName = "NewIntReactiveVariable", menuName = "DataStructures/ReactiveVariables/Int")]
    public class IntReactiveVariable : AbstractReactiveVariable<int>
    {
        public void Add(int value)
        {
            runtimeProperty.Value += value;
        }

        public void Add(IntReactiveVariable reactiveVariable)
        {
            runtimeProperty.Value += reactiveVariable.RuntimeProperty.Value;
        }
    }
}
