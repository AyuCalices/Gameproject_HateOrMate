using System.Collections;
using DataStructures.Event;
using ExitGames.Client.Photon;
using Unity.Collections;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Features.Battle.StateMachine
{
    public abstract class BaseBattleState_SO : ScriptableObject, IBattleState
    {
        public ActionEvent onEnterState;
        public ActionEvent onExitState;
        public bool isStateActive;
        
        public virtual IEnumerator Enter()
        {
            onEnterState?.Raise();
            isStateActive = true;
            yield break;
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
