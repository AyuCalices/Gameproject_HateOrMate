using DataStructures.Variables;
using UnityEngine;

namespace DataStructures.StateLogic
{
    [CreateAssetMenu(fileName = "newStateData", menuName = "DataStructures/StateLogic/StateData")]
    public class StateData : ScriptableObject
    {
        [SerializeField] private BoolVariable isStateActive;
        
        // Enter-/Exit-Events of states
        [SerializeField] private OnStateEnterEvent onEnterEventEvent;
        [SerializeField] private OnStateExitEvent onExitEventEvent;

        public BoolVariable IsStateActive => isStateActive;

        public OnStateEnterEvent OnEnterEventEvent => onEnterEventEvent;
        public OnStateExitEvent OnExitEventEvent => onExitEventEvent;
    }
}