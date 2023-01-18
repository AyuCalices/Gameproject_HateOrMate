using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UniRx;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Features.Battle.StateMachine
{
    public class BattleStateMachine
    {
        public IBattleState CurrentState { get; private set; }

        private readonly List<Func<IEnumerator>> _queue = new List<Func<IEnumerator>>();
        
        public void Initialize(IBattleState startingState)
        {
            _queue.Add(() => Enter(startingState));
            
            Observable.FromCoroutine(() => _queue.Count > 0 ? _queue[0].Invoke() : Break())
                .DoOnCompleted(() =>
                {
                    if (_queue.Count > 0) _queue.RemoveAt(0);
                })
                .Repeat()
                .Subscribe();
        }

        public void ChangeState(IBattleState newState)
        {
            _queue.Add(() => Exit(CurrentState));
            _queue.Add(() => Enter(newState));
        }

        private IEnumerator Break()
        {
            yield return null;
        }
        
        private IEnumerator Enter(IBattleState newState)
        {
            CurrentState = newState;
            yield return Observable.FromCoroutine(newState.Enter).ToYieldInstruction();
        }
        
        private IEnumerator Exit(IBattleState newState)
        {
            yield return Observable.FromCoroutine(newState.Exit).ToYieldInstruction();
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
