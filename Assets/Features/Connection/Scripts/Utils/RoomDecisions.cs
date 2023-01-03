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

        public T GetDecisionValue(Player player)
        {
            string identifier = Identifier(player);
            return (T)PhotonNetwork.CurrentRoom.CustomProperties[identifier];
        }

        public RoomDecisions(string identifier, bool triggerIfOneChose)
        {
            _identifier = identifier;
            _triggerIfOneChose = triggerIfOneChose;
        }
        
        public bool OverwriteLocalDecision(T value)
        {
            Player localPlayer = PhotonNetwork.LocalPlayer;

            _localDecision = new Hashtable(){{Identifier(localPlayer), value}};
            PhotonNetwork.CurrentRoom.SetCustomProperties(_localDecision);

            return true;
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

        public bool UpdateDecision(Action onAllPlayerChose, Predicate<T> predicate = null)
        {
            if (_triggerIfOneChose)
            {
                if (!AnyPlayerChose(predicate)) return false;
            }
            else
            {
                if (!PlayerChose(predicate)) return false;
            }
            
            onAllPlayerChose.Invoke();
            ResetLocalDecision();
            return true;
        }
        
        private bool AnyPlayerChose(Predicate<T> predicate)
        {
            Hashtable roomCustomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
            
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (!roomCustomProperties.ContainsKey(Identifier(player)) ||
                    roomCustomProperties[Identifier(player)] == null) continue;

                return predicate == null || predicate.Invoke((T)roomCustomProperties[Identifier(player)]);
            }

            return false;
        }
        
        private bool PlayerChose(Predicate<T> predicate)
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
                
                if (predicate != null && !predicate.Invoke((T)roomCustomProperties[Identifier(player)]))
                {
                    return false;
                }
            }

            return true;
        }

        public void ResetLocalDecision()
        {
            Player localPlayer = PhotonNetwork.LocalPlayer;
            _localDecision = new Hashtable(){{Identifier(localPlayer), null}};
            PhotonNetwork.CurrentRoom.SetCustomProperties(_localDecision);
        }
    }
}
