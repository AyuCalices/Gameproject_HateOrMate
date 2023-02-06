using DataStructures.Event;
using ExitGames.Client.Photon;
using Features.Connection.Scripts.Utils;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace Features.Connection.Scripts.View
{
    public class RoomViewBehaviour : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameEvent onJoinedRoom;
        [SerializeField] private GameEvent onLeftRoom;
    
        [SerializeField] private PlayerRoomUnitInstanceBehaviour playerRoomUnitInstanceBehaviourPrefab;
        [SerializeField] private Transform playerRoomBehaviourParent;
        [SerializeField] private TMP_Text roomName;
        [SerializeField] private BoolRoomDecitions_SO readyCheckRoomDecision;

        private bool _isInLobby;
        private bool _isReady;

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            Debug.Log("o/");
            UpdatePlayerDecisionVisualisation();
            
            if (!PhotonNetwork.IsMasterClient) return;
            
            readyCheckRoomDecision.IsValidDecision(() => PhotonNetwork.LoadLevel("GameScene"), b => b);
        }

        public void StartGame()
        {
            _isReady = !_isReady;
            readyCheckRoomDecision.SetDecision(_isReady);
        }

        private void UpdatePlayerDecisionVisualisation()
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                string identifier = readyCheckRoomDecision.Identifier(player);

                if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(identifier))
                {
                    PlayerRoomUnitInstanceBehaviour.SetReadyButton(player, false);
                }
                else if (PhotonNetwork.CurrentRoom.CustomProperties[identifier] is bool decisionValue)
                {
                    PlayerRoomUnitInstanceBehaviour.SetReadyButton(player, decisionValue);
                }
            }
        }

        public override void OnJoinedRoom()
        {
            onJoinedRoom.Raise();
            
            roomName.text = "Room Code: " + PhotonNetwork.CurrentRoom.Name;

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                playerRoomUnitInstanceBehaviourPrefab.Instantiate(playerRoomBehaviourParent, player);
            }
        }
    
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
            
            playerRoomUnitInstanceBehaviourPrefab.Instantiate(playerRoomBehaviourParent, newPlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            readyCheckRoomDecision.ResetDecisions();
            PlayerRoomUnitInstanceBehaviour.Destroy(otherPlayer);
        }

        public override void OnLeftRoom()
        {
            onLeftRoom.Raise();
            
            readyCheckRoomDecision = null;
            PlayerRoomUnitInstanceBehaviour.DestroyAll();
        }
    }
}
