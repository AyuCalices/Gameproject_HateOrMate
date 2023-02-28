using System;
using Features.Connection.Scripts.Utils;
using Features.Music;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Features.Connection.Scripts.View
{
    public enum JoinType { KeyJoin = 0, CreateRoom = 1, QuickJoin = 2 }
    public class ConnectionViewBehaviour : MonoBehaviourPunCallbacks
    {
        [SerializeField] private ErrorPopup errorPopup;
        [SerializeField] private Transform errorPopupParent;
        [SerializeField] private TMP_InputField roomKeyInput;
        [SerializeField] private MusicBehaviour musicBehaviour;
        [SerializeField] private UnityEvent onDisconnect;

        private JoinType _selectedJoinType;

        private void Awake()
        {
            musicBehaviour.Enable();
        }

        public void OnClickConnect(int joinType)
        {
            if (roomKeyInput.text.Length == 0 && (JoinType) joinType is JoinType.KeyJoin)
            {
                errorPopup.Instantiate(errorPopupParent, "You need to add a Room Code to do that!", OnClickDisconnect);
                return;
            }
            
            _selectedJoinType = (JoinType)joinType;
            
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.NickName = LobbyRoomNameRandomizer.RandomString(5);
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            onDisconnect.Invoke();
        }

        public void OnClickDisconnect()
        {
            if (SceneManager.GetActiveScene().name == "GameScene")
            {
                SceneManager.LoadScene("ConnectionScreen");
            }
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

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            base.OnCreateRoomFailed(returnCode, message);
            errorPopup.Instantiate(errorPopupParent, returnCode + ": " + message);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            base.OnJoinRoomFailed(returnCode, message);
            errorPopup.Instantiate(errorPopupParent, returnCode + ": " + message);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            base.OnJoinRandomFailed(returnCode, message);
            errorPopup.Instantiate(errorPopupParent, returnCode + ": " + message);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public override void OnErrorInfo(ErrorInfo errorInfo)
        {
            base.OnErrorInfo(errorInfo);
            //TODO: check what to implement
        }
    }
}
