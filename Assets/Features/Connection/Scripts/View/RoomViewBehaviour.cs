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

        private RoomDecisions<bool> _roomDecisions;
        private bool _isInLobby;
        private bool _isReady;

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            UpdatePlayerDecisionVisualisation();
            
            if (!PhotonNetwork.IsMasterClient) return;
            
            _roomDecisions.IsValidDecision(() => PhotonNetwork.LoadLevel("GameScene"), b => b);
        }

        public void StartGame()
        {
            _isReady = !_isReady;
            _roomDecisions.SetLocalDecision(_isReady);
        }

        private void UpdatePlayerDecisionVisualisation()
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                string identifier = _roomDecisions.Identifier(player);

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

            _roomDecisions = new RoomDecisions<bool>("Lobby", false);
        }
    
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
            
            playerRoomUnitInstanceBehaviourPrefab.Instantiate(playerRoomBehaviourParent, newPlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            _roomDecisions.ResetLocalDecision();
            PlayerRoomUnitInstanceBehaviour.Destroy(otherPlayer);
        }

        public override void OnLeftRoom()
        {
            onLeftRoom.Raise();
            
            _roomDecisions = null;
            PlayerRoomUnitInstanceBehaviour.DestroyAll();
        }
    }
}
