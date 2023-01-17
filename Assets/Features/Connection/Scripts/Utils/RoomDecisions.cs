using System;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Features.Connection.Scripts.Utils
{
    public class RoomDecisions<T>
    {
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

        public void SetLocalDecision(T value)
        {
            Player localPlayer = PhotonNetwork.LocalPlayer;
            
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable(){{Identifier(localPlayer), value}});
        }
        
        public bool IsValidDecision(Action onValidDecision = null, Predicate<T> predicate = null)
        {
            if (_triggerIfOneChose)
            {
                if (!AnyPlayerChose(predicate)) return false;
            }
            else
            {
                if (!AllPlayerChose(predicate)) return false;
            }
            
            onValidDecision?.Invoke();
            ResetLocalDecision();
            return true;
        }
        
        private bool AnyPlayerChose(Predicate<T> predicate)
        {
            Hashtable roomCustomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
            
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (!roomCustomProperties.ContainsKey(Identifier(player))) continue;

                return predicate == null || predicate.Invoke((T)roomCustomProperties[Identifier(player)]);
            }

            return false;
        }
        
        private bool AllPlayerChose(Predicate<T> predicate)
        {
            Hashtable roomCustomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
            
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (!roomCustomProperties.ContainsKey(Identifier(player)))
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
            PhotonNetwork.CurrentRoom.CustomProperties.Remove(Identifier(localPlayer));
        }
    }
}
