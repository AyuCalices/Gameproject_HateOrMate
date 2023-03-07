using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Features.General.Photon.Scripts
{
    public abstract class BaseRoomDecisions_SO<T> : ScriptableObject
    {
        [field: SerializeField] private string identifier;
        [field: SerializeField] private bool triggerIfOneChose;

        public string Identifier(Player player) => identifier + player.ActorNumber;

        public bool TryGetDecisionValue(Player player, out T value)
        {
            string identifier = Identifier(player);

            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(identifier))
            {
                value = (T) PhotonNetwork.CurrentRoom.CustomProperties[identifier];
                return true;
            }

            value = default;
            return false;
        }

        public void SetDecision(T value)
        {
            Player localPlayer = PhotonNetwork.LocalPlayer;
            
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable(){{Identifier(localPlayer), value}});
        }
        
        public bool IsValidDecision(Action onValidDecision = null, Predicate<T> predicate = null)
        {
            if (triggerIfOneChose)
            {
                if (!AnyPlayerChose(predicate)) return false;
            }
            else
            {
                if (!AllPlayerChose(predicate)) return false;
            }
            
            onValidDecision?.Invoke();
            ResetDecisions();
            return true;
        }
        
        private bool AnyPlayerChose(Predicate<T> predicate)
        {
            Hashtable roomCustomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
            
            foreach (Player currentRoomPlayer in PhotonNetwork.PlayerList)
            {
                if (!roomCustomProperties.ContainsKey(Identifier(currentRoomPlayer))) continue;

                return predicate == null || predicate.Invoke((T)roomCustomProperties[Identifier(currentRoomPlayer)]);
            }

            return false;
        }
        
        private bool AllPlayerChose(Predicate<T> predicate)
        {
            Hashtable roomCustomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
            
            foreach (Player currentRoomPlayer in PhotonNetwork.PlayerList)
            {
                if (!roomCustomProperties.ContainsKey(Identifier(currentRoomPlayer)))
                {
                    return false;
                }
                
                if (predicate != null && !predicate.Invoke((T)roomCustomProperties[Identifier(currentRoomPlayer)]))
                {
                    return false;
                }
            }

            return true;
        }

        public void ResetDecisions()
        {
            Hashtable roomCustomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
            
            foreach (Player currentRoomPlayer in PhotonNetwork.PlayerList)
            {
                if (!roomCustomProperties.ContainsKey(Identifier(currentRoomPlayer))) continue;
                PhotonNetwork.CurrentRoom.CustomProperties.Remove(Identifier(currentRoomPlayer));
            }
        }
    }
}
