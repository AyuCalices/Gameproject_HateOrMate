using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class PlayerRoomBehaviour : MonoBehaviour
{
    [SerializeField] private TMP_Text tmpDescription;

    private Player _playerReference;

    private static List<PlayerRoomBehaviour> _playerRoomBehaviours = new List<PlayerRoomBehaviour>();

    public void Instantiate(Transform instantiationParent, Player player)
    {
        PlayerRoomBehaviour playerRoomBehaviour = Instantiate(this, instantiationParent);
        playerRoomBehaviour.tmpDescription.text = Equals(player, PhotonNetwork.LocalPlayer) ? "You" : "Mate";
        playerRoomBehaviour._playerReference = player;
        _playerRoomBehaviours.Add(playerRoomBehaviour);
    }

    public static void Destroy(Player playerReference)
    {
        for (int index = _playerRoomBehaviours.Count - 1; index >= 0; index--)
        {
            PlayerRoomBehaviour playerRoomBehaviour = _playerRoomBehaviours[index];
            if (Equals(playerRoomBehaviour._playerReference, playerReference))
            {
                _playerRoomBehaviours.Remove(playerRoomBehaviour);
                Destroy(playerRoomBehaviour.gameObject);
            }
        }
    }

    public static void DestroyAll()
    {
        for (int index = _playerRoomBehaviours.Count - 1; index >= 0; index--)
        {
            PlayerRoomBehaviour playerRoomBehaviour = _playerRoomBehaviours[index];
            Destroy(playerRoomBehaviour.gameObject);
        }
        
        _playerRoomBehaviours.Clear();
    }
}
