using System;
using DataStructures.Event;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Features.Connection.Scripts.View
{
    public class RoomViewBehaviour : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameEvent onJoinedRoom;
        [SerializeField] private GameEvent onLeftRoom;
    
        [SerializeField] private PlayerRoomUnitInstanceBehaviour playerRoomUnitInstanceBehaviourPrefab;
        [SerializeField] private Transform playerRoomBehaviourParent;
        [SerializeField] private TMP_Text roomName;

        public void StartGame()
        {
            PhotonNetwork.LoadLevel("GameScene");
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
            PlayerRoomUnitInstanceBehaviour.Destroy(otherPlayer);
        }

        public override void OnLeftRoom()
        {
            onLeftRoom.Raise();
            
            PlayerRoomUnitInstanceBehaviour.DestroyAll();
        }
    }
}
