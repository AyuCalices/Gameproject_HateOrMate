using System;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace Features.Connection
{
    public enum JoinType { KeyJoin = 0, CreateRoom = 1, QuickJoin = 2 }
    public class ConnectToRoom : MonoBehaviourPunCallbacks
    {
        [SerializeField] private ErrorPopup errorPopup;
        [SerializeField] private Transform errorPopupParent;

        [SerializeField] private PlayerRoomBehaviour playerRoomBehaviourPrefab;
        [SerializeField] private Transform playerRoomBehaviourParent;
        
        public GameObject loadingView;
        public GameObject mainView;
        public GameObject roomView;
        
        public TMP_Text roomName;
        public TMP_InputField roomKeyInput;

        private JoinType _selectedJoinType;

        private void Awake()
        {
            loadingView.SetActive(false);
            mainView.SetActive(true);
            roomView.SetActive(false);
        }
        
        public void OnClickConnect(int joinType)
        {
            if (roomKeyInput.text.Length == 0 && (JoinType) joinType is JoinType.KeyJoin)
            {
                errorPopup.Instantiate(errorPopupParent, "You need to add a Room Code to do that!");
                return;
            }
            
            _selectedJoinType = (JoinType)joinType;
            
            PhotonNetwork.AutomaticallySyncScene = true;
            loadingView.SetActive(true);
            PhotonNetwork.NickName = LobbyRoomNameRandomizer.RandomString(5);
            PhotonNetwork.ConnectUsingSettings();
        }

        public void StartGame()
        {
            PhotonNetwork.LoadLevel(2);
        }
        
        public void OnClickDisconnect()
        {
            PhotonNetwork.Disconnect();
        }

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            RoomOptions roomOptions;
            switch (_selectedJoinType)
            {
                case JoinType.KeyJoin:
                    PhotonNetwork.JoinRoom(roomKeyInput.text);
                    break;
                case JoinType.CreateRoom:
                    roomOptions = new RoomOptions()
                        {MaxPlayers = 2, EmptyRoomTtl = 0, IsVisible = false, BroadcastPropsChangeToAll = true};
                    PhotonNetwork.CreateRoom(LobbyRoomNameRandomizer.RandomString(4), roomOptions);
                    break;
                case JoinType.QuickJoin:
                    roomOptions = new RoomOptions()
                        {MaxPlayers = 2, EmptyRoomTtl = 0, IsVisible = true, BroadcastPropsChangeToAll = true};
                    PhotonNetwork.JoinRandomOrCreateRoom(null, 2, MatchmakingMode.FillRoom, 
                        null, null, LobbyRoomNameRandomizer.RandomString(4), roomOptions, null);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void OnJoinedRoom()
        {
            loadingView.SetActive(false);
            roomView.SetActive(true);
            roomName.text = "Room Code: " + PhotonNetwork.CurrentRoom.Name;

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                playerRoomBehaviourPrefab.Instantiate(playerRoomBehaviourParent, player);
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
            
            playerRoomBehaviourPrefab.Instantiate(playerRoomBehaviourParent, newPlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log("o/");
            PlayerRoomBehaviour.Destroy(otherPlayer);
        }

        public override void OnLeftRoom()
        {
            mainView.SetActive(true);
            roomView.SetActive(false);
            PlayerRoomBehaviour.DestroyAll();
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            base.OnCreateRoomFailed(returnCode, message);
            errorPopup.Instantiate(errorPopupParent, returnCode + ": " + message, () =>
            {
                mainView.SetActive(true);
                loadingView.SetActive(false);
                PhotonNetwork.Disconnect();
            });
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            base.OnJoinRoomFailed(returnCode, message);
            errorPopup.Instantiate(errorPopupParent, returnCode + ": " + message, () =>
            {
                mainView.SetActive(true);
                loadingView.SetActive(false);
                PhotonNetwork.Disconnect();
            });
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            base.OnJoinRandomFailed(returnCode, message);
            errorPopup.Instantiate(errorPopupParent, returnCode + ": " + message, () =>
            {
                mainView.SetActive(true);
                loadingView.SetActive(false);
                PhotonNetwork.Disconnect();
            });
        }

        public override void OnErrorInfo(ErrorInfo errorInfo)
        {
            base.OnErrorInfo(errorInfo);
            //TODO: check what to implement
        }
    }
}
