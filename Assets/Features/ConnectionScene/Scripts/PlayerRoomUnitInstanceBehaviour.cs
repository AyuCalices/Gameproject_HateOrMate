using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace Features.Connection.Scripts
{
    public class PlayerRoomUnitInstanceBehaviour : MonoBehaviour
    {
        [SerializeField] private TMP_Text tmpDescription;
        [SerializeField] private GameObject readyImage;

        private Player _playerReference;

        private static List<PlayerRoomUnitInstanceBehaviour> _playerRoomBehaviours = new List<PlayerRoomUnitInstanceBehaviour>();

        public void Instantiate(Transform instantiationParent, Player player)
        {
            PlayerRoomUnitInstanceBehaviour playerRoomUnitInstanceBehaviour = Instantiate(this, instantiationParent);
            playerRoomUnitInstanceBehaviour.tmpDescription.text = Equals(player, PhotonNetwork.LocalPlayer) ? "You" : "Mate";
            playerRoomUnitInstanceBehaviour._playerReference = player;
            playerRoomUnitInstanceBehaviour.readyImage.SetActive(false);
            _playerRoomBehaviours.Add(playerRoomUnitInstanceBehaviour);
        }

        private void OnDisable()
        {
            _playerRoomBehaviours.Remove(this);
            Destroy(gameObject);
        }

        public static void Destroy(Player playerReference)
        {
            for (int index = _playerRoomBehaviours.Count - 1; index >= 0; index--)
            {
                PlayerRoomUnitInstanceBehaviour playerRoomUnitInstanceBehaviour = _playerRoomBehaviours[index];
                if (Equals(playerRoomUnitInstanceBehaviour._playerReference, playerReference))
                {
                    _playerRoomBehaviours.Remove(playerRoomUnitInstanceBehaviour);
                    Destroy(playerRoomUnitInstanceBehaviour.gameObject);
                }
            }
            
            _playerRoomBehaviours.Clear();
        }

        public static void SetReadyButton(Player playerReference, bool value)
        {
            for (int index = _playerRoomBehaviours.Count - 1; index >= 0; index--)
            {
                PlayerRoomUnitInstanceBehaviour playerRoomUnitInstanceBehaviour = _playerRoomBehaviours[index];
                if (Equals(playerRoomUnitInstanceBehaviour._playerReference, playerReference))
                {
                    playerRoomUnitInstanceBehaviour.readyImage.SetActive(value);
                }
            }
        }
    }
}
