using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace Features.Connection
{
    public class LobbyManager : MonoBehaviourPunCallbacks
    {
        public GameObject lobbyPanel;
        public GameObject roomPanel;
        public TMP_Text roomName;
        public RoomItem roomItemPrefab;
        public Transform contentObject;

        [SerializeField] private bool joinRandom;
    
        public TMP_InputField joinRoomInputField;

        public float timeBetweenUpdates = 1.5f;

        private float timeBetweenUpdatesDelta;
    
        private List<RoomItem> roomItemsList = new List<RoomItem>();

        private void Start()
        {
            PhotonNetwork.JoinLobby();
        }

        public void OnClickCreate()
        {
            PhotonNetwork.CreateRoom(LobbyRoomNameRandomizer.RandomString(5), new RoomOptions() {MaxPlayers = 2, EmptyRoomTtl = 0, IsVisible = false, BroadcastPropsChangeToAll = true});
        }

        public override void OnJoinedRoom()
        {
            lobbyPanel.SetActive(false);
            roomPanel.SetActive(true);
            roomName.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name;
        }
    
        public void MoveToGameScene()
        {
            PhotonNetwork.LoadLevel(2);
        }
    
        public override void OnLeftRoom()
        {
            lobbyPanel.SetActive(true);
            roomPanel.SetActive(false);
        }

        public override void OnJoinedLobby()
        {
            if (joinRandom)
            {
                PhotonNetwork.JoinRandomOrCreateRoom();
            }
        }

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinLobby();
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            //Debug.Log("o/");
        
            if (Time.time >= timeBetweenUpdatesDelta)
            {
                UpdateRoomList(roomList);
                timeBetweenUpdatesDelta = Time.time + timeBetweenUpdates;
            }
        }
    
        public void JoinRoom()
        {
            if (joinRoomInputField.text.Length >= 1)
            {
                Debug.Log(joinRoomInputField.text);
                PhotonNetwork.JoinRoom(joinRoomInputField.text);
            }
        }

        public void JoinRoom(string roomName)
        {
            PhotonNetwork.JoinRoom(roomName);
        }

        public void OnClickLeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        private void UpdateRoomList(List<RoomInfo> list)
        {
            foreach (RoomItem item in roomItemsList)
            {
                Destroy(item.gameObject);
            }
            roomItemsList.Clear();

            foreach (RoomInfo room in list)
            {
                if (room.RemovedFromList)
                {
                    return;
                }
            
                RoomItem newRoom = Instantiate(roomItemPrefab, contentObject);
                newRoom.SetRoomName(room.Name);
                roomItemsList.Add(newRoom);
            }
        }
    }
}
