using DataStructures.Variables;
using UnityEngine;

namespace DataStructures.StateLogic
{
    [CreateAssetMenu(fileName = "newStateData", menuName = "DataStructures/StateLogic/StateData")]
    public class StateData : ScriptableObject
    {
        [SerializeField] private BoolVariable isStateActive;
        
        // Enter-/Exit-Events of states
        [SerializeField] private OnStateEnterEvent onEnterEvent;
        [SerializeField] private OnStateExitEvent onExitEvent;

        public BoolVariable IsStateActive => isStateActive;

        public OnStateEnterEvent OnEnterEvent => onEnterEvent;
        public OnStateExitEvent OnExitEvent => onExitEvent;
    }
}