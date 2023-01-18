using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UniRx;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Features.Battle.StateMachine
{
    public class CoroutineStateMachine
    {
        public ICoroutineState CurrentState { get; private set; }

        private readonly List<Func<IEnumerator>> _queue = new List<Func<IEnumerator>>();
        
        public void Initialize(ICoroutineState startingState)
        {
            _queue.Add(() => Enter(startingState));
            
            Observable.FromCoroutine(() => _queue.Count > 0 ? _queue[0].Invoke() : Execute())
                .Repeat()
                .Subscribe();
        }

        public void ChangeState(ICoroutineState newState)
        {
            _queue.Add(Exit);
            _queue.Add(() => Enter(newState));
        }

        private IEnumerator Execute()
        {
            yield return Observable.FromCoroutine(CurrentState.Execute).ToYieldInstruction();
        }
        
        private IEnumerator Enter(ICoroutineState newState)
        {
            CurrentState = newState;
            _queue.RemoveAt(0);
            yield return Observable.FromCoroutine(newState.Enter).ToYieldInstruction();
        }
        
        private IEnumerator Exit()
        {
            _queue.RemoveAt(0);
            yield return Observable.FromCoroutine(CurrentState.Exit).ToYieldInstruction();
        }

        public void OnRoomPropertiesUpdated(Hashtable propertiesThatChanged)
        {
            CurrentState?.OnRoomPropertiesUpdate(propertiesThatChanged);
        }

        public void OnEvent(EventData photonEvent)
        {
            CurrentState?.OnEvent(photonEvent);
        }
    }
}
