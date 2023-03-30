using System.Collections;
using DataStructures.Event;
using Features.General.Photon.Scripts;
using Features.Music.Scripts;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Features.ConnectionScene.Scripts.View
{
    public class RoomViewBehaviour : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameEvent onJoinedRoom;
        [SerializeField] private GameEvent onLeftRoom;
        [SerializeField] private int expectedPlayerCount = 1;
    
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
            
            if (!PhotonNetwork.IsMasterClient || PhotonNetwork.CurrentRoom.PlayerCount < expectedPlayerCount) return;
            
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
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                string identifier = readyCheckRoomDecision.UsageIdentifier(player);
                Debug.Log(identifier);

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

            playerCount.text = PhotonNetwork.CurrentRoom.PlayerCount + " / " + expectedPlayerCount;
            UpdatePlayerDecisionVisualisation();
        }
    
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            playerCount.text = PhotonNetwork.CurrentRoom.PlayerCount + " / " + expectedPlayerCount;
            playerRoomUnitInstanceBehaviourPrefab.Instantiate(playerRoomBehaviourParent, newPlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            playerCount.text = PhotonNetwork.CurrentRoom.PlayerCount + " / " + expectedPlayerCount;
            PlayerRoomUnitInstanceBehaviour.Destroy(otherPlayer);
        }

        public override void OnLeftRoom()
        {
            onLeftRoom.Raise();
        }
    }
}
