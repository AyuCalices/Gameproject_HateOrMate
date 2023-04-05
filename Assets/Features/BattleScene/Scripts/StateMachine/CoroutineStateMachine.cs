using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UniRx;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Features.BattleScene.Scripts.StateMachine
{
    public enum StateProgressType { Enter, Execute, Exit, All }
    public class CoroutineStateMachine
    {
        private ICoroutineState _currentState;
        private StateProgressType _stageProgressType;

        private readonly List<Func<IEnumerator>> _queue = new List<Func<IEnumerator>>();

        public bool StateIsValid(Type checkedType, StateProgressType checkedStateProgressType)
        {
            return _currentState.GetType() == checkedType && (_stageProgressType == checkedStateProgressType || _stageProgressType == StateProgressType.All);
        }
        
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
            _stageProgressType = StateProgressType.Execute;
            yield return Observable.FromCoroutine(_currentState.Execute).ToYieldInstruction();
        }
        
        private IEnumerator Enter(ICoroutineState newState)
        {
            _currentState = newState;
            _stageProgressType = StateProgressType.Enter;
            _queue.RemoveAt(0);
            yield return Observable.FromCoroutine(newState.Enter).ToYieldInstruction();
        }
        
        private IEnumerator Exit()
        {
            _stageProgressType = StateProgressType.Exit;
            _queue.RemoveAt(0);
            yield return Observable.FromCoroutine(_currentState.Exit).ToYieldInstruction();
        }

        public void OnRoomPropertiesUpdated(Hashtable propertiesThatChanged)
        {
            _currentState?.OnRoomPropertiesUpdate(propertiesThatChanged);
        }

        public void OnEvent(EventData photonEvent)
        {
            _currentState?.OnEvent(photonEvent);
        }
    }
}
