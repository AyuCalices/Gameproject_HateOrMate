using System;
using DataStructures.Event;
using Features.Connection.Scripts.Utils;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UniRx;
using UniRx.Triggers;
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

        private IDisposable _updatePlayerDecisionDisposable;
        private IDisposable _updateDecisions;

        public override void OnEnable()
        {
            base.OnEnable();
            _updatePlayerDecisionDisposable = this.UpdateAsObservable()
                .SampleFrame(4)
                .Where(_ => _roomDecisions != null)
                .Subscribe(_ => UpdatePlayerDecisionVisualisation());
            
            _updateDecisions = this.UpdateAsObservable()
                .SampleFrame(4)
                .Where(_ => PhotonNetwork.IsMasterClient && _isReady)
                .Subscribe(_ => _roomDecisions.UpdateDecision(() => PhotonNetwork.LoadLevel("GameScene"), b => b));
        }

        public override void OnDisable()
        {
            base.OnDisable();
            _updatePlayerDecisionDisposable.Dispose();
            _updateDecisions.Dispose();
        }

        public void StartGame()
        {
            _isReady = !_isReady;
            _roomDecisions.OverwriteLocalDecision(_isReady);
        }

        private void UpdatePlayerDecisionVisualisation()
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                string identifier = _roomDecisions.Identifier(player);

                if (PhotonNetwork.CurrentRoom.CustomProperties[identifier] is bool decisionValue)
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
