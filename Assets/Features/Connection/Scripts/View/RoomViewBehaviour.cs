using System.Collections;
using DataStructures.Event;
using Features.Connection.Scripts.Utils;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

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
        [SerializeField] private MusicBehaviour musicBehaviour;
        [SerializeField] private TMP_Text playerCount;

        private bool _isInLobby;
        private bool _isReady;

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            UpdatePlayerDecisionVisualisation();
            
            if (!PhotonNetwork.IsMasterClient || PhotonNetwork.CurrentRoom.PlayerCount < 2) return;
            
            readyCheckRoomDecision.IsValidDecision(() =>
            {
                musicBehaviour.Disable();
                StartCoroutine(OnStartGame());
            }, b => b);
        }

        private IEnumerator OnStartGame()
        {
            yield return new WaitForSeconds(musicBehaviour.MusicFadeTime);
            PhotonNetwork.LoadLevel("GameScene");
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }

        public void StartGame()
        {
            _isReady = !_isReady;
            readyCheckRoomDecision.SetDecision(_isReady);
        }

        private void UpdatePlayerDecisionVisualisation()
        {
            playerCount.text = PhotonNetwork.CurrentRoom.PlayerCount + " / 2"; 
            
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

            UpdatePlayerDecisionVisualisation();
        }
    
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            playerRoomUnitInstanceBehaviourPrefab.Instantiate(playerRoomBehaviourParent, newPlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            PlayerRoomUnitInstanceBehaviour.Destroy(otherPlayer);
        }

        public override void OnLeftRoom()
        {
            onLeftRoom.Raise();
        }
    }
}
