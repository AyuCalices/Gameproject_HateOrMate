using UniRx;
using UnityEngine;

namespace DataStructures.ReactiveVariable
{
    public class AbstractReactiveVariable<T> : ScriptableObject
    {
        [SerializeField] protected ReactiveProperty<T> runtimeProperty;
        [SerializeField] protected T storedValue;

        public ReactiveProperty<T> RuntimeProperty => runtimeProperty;
    
        private void OnEnable()
        {
            Restore(false);
        }

        public void Restore(bool keepSubscriptions)
        {
            if (keepSubscriptions && runtimeProperty != null)
            {
                runtimeProperty.Value = storedValue;
            }
            else
            {
                runtimeProperty = new ReactiveProperty<T>(storedValue);
            }
        }

        public T Get() => runtimeProperty.Value;

        public void Set(T value)
        {
            if (value.Equals(runtimeProperty.Value)) return;
            
            runtimeProperty.Value = value;
        }
    }
}
