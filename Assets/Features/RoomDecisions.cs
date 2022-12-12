using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Features
{
    public class RoomDecisions<T>
    {
        private Hashtable _localDecision;
        private readonly string _identifier;
    
        public string Identifier(Player player) => _identifier + player.ActorNumber;

        public RoomDecisions(string identifier)
        {
            _identifier = identifier;
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

        public void UpdateDecision(Action onAllPlayerChose)
        {
            if (!AllPlayerChose()) return;
            onAllPlayerChose.Invoke();
            ResetLocalDecision();
        }
        
        private bool AllPlayerChose()
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
