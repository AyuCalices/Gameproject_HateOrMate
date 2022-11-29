using DataStructures.Event;
using UnityEngine;

namespace DataStructures.Focus
{
    public abstract class Focus_SO<T> : ScriptableObject
    {
        protected T Focus;
        
        [SerializeField] private GameEvent onFocusChanged;
        
        public void Restore()
        {
            Focus = default;
            if(onFocusChanged != null) onFocusChanged.Raise();
        }

        public bool NotNull()
        {
            return Focus != null;
        }
        
        public T Get()
        {
            return Focus;
        }
        
        public void Set(T value)
        {
            Focus = value;
            if(onFocusChanged != null) onFocusChanged.Raise();
        }
    }
}
