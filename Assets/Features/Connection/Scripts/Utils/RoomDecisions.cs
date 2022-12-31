using System;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Features.Connection.Scripts.Utils
{
    public class RoomDecisions<T>
    {
        private Hashtable _localDecision;
        private readonly string _identifier;
        private readonly bool _triggerIfOneChose;

        public string Identifier(Player player) => _identifier + player.ActorNumber;

        public RoomDecisions(string identifier, bool triggerIfOneChose)
        {
            _identifier = identifier;
            _triggerIfOneChose = triggerIfOneChose;
        }

        public bool SetLocalDecision(T value)
        {
            Player localPlayer = PhotonNetwork.LocalPlayer;
            
            Hashtable roomCustomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
            if (roomCustomProperties[Identifier(localPlayer)] != null) return false;

            _localDecision = new Hashtable(){{Identifier(localPlayer), value}};
            PhotonNetwork.CurrentRoom.SetCustomProperties(_localDecision);

            return true;
        }

        public bool UpdateDecision(Action onAllPlayerChose)
        {
            if (_triggerIfOneChose)
            {
                if (!AnyPlayerChose()) return false;
            }
            else
            {
                if (!PlayerChose()) return false;
            }
            
            onAllPlayerChose.Invoke();
            ResetLocalDecision();
            return true;
        }

        private bool AnyPlayerChose()
        {
            Hashtable roomCustomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
            
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (roomCustomProperties.ContainsKey(Identifier(player)) && roomCustomProperties[Identifier(player)] != null)
                {
                    return true;
                }
            }

            return false;
        }
        
        private bool PlayerChose()
        {
            Hashtable roomCustomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
            
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (Equals(player, PhotonNetwork.LocalPlayer) && _localDecision != null && _localDecision[Identifier(player)] == null)
                {
                    return false;
                }
                
                if (!roomCustomProperties.ContainsKey(Identifier(player)) || roomCustomProperties[Identifier(player)] == null)
                {
                    return false;
                }
            }

            return true;
        }

        private void ResetLocalDecision()
        {
            Player localPlayer = PhotonNetwork.LocalPlayer;
            _localDecision = new Hashtable(){{Identifier(localPlayer), null}};
            PhotonNetwork.CurrentRoom.SetCustomProperties(_localDecision);
        }
    }
}
