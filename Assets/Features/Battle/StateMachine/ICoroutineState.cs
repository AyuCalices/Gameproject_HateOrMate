using System.Collections;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Features.Battle.StateMachine
{
    public interface ICoroutineState
    {
        IEnumerator Enter();

        IEnumerator Execute();
    
        IEnumerator Exit();

        void OnEvent(EventData photonEvent);

        void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged);
    }
}
