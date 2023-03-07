using System.Collections;
using DataStructures.Event;
using ExitGames.Client.Photon;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Features.BattleScene.Scripts.StateMachine
{
    public abstract class BaseCoroutineState : ScriptableObject, ICoroutineState
    {
        [Header("Base Coroutine References")]
        [SerializeField] private ActionEvent onEnterState;
        [SerializeField] private ActionEvent onExitState;
        [SerializeField] private bool isStateActive;

        public virtual IEnumerator Enter()
        {
            onEnterState?.Raise();
            isStateActive = true;
            yield break;
        }

        public virtual IEnumerator Execute()
        {
            yield return null;
        }

        public virtual IEnumerator Exit()
        {
            onExitState?.Raise();
            isStateActive = false;
            yield break;
        }

        public virtual void OnEvent(EventData photonEvent) { }

        public virtual void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) { }
    }
}