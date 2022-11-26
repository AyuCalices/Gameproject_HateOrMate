using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor;
using UnityEngine;

namespace Features.Unit
{
    public class UnitSpawner : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        [SerializeField] private LocalUnitBehaviour localPlayerPrefab;
        [SerializeField] private NetworkedUnitBehaviour networkedPlayerPrefab;
        [SerializeField] private byte customManualInstantiationEventCode;
        
        public void Spawn()
        {
            SpawnPlayer();
            //PhotonNetwork.Instantiate("Unit", new Vector3(Random.Range(0, 5), Random.Range(0, 5), 0), Quaternion.identity, 0);
        }
        
        public void SpawnPlayer()
        {
            GameObject player = Instantiate(localPlayerPrefab.gameObject);
            player.transform.position = new Vector3(Random.Range(0, 5), Random.Range(0, 5), 0);
            PhotonView photonView = player.GetComponent<PhotonView>();

            if (PhotonNetwork.AllocateViewID(photonView))
            {
                object[] data = new object[]
                {
                    player.transform.position, player.transform.rotation, photonView.ViewID
                };

                RaiseEventOptions raiseEventOptions = new RaiseEventOptions
                {
                    Receivers = ReceiverGroup.Others,
                    CachingOption = EventCaching.AddToRoomCache
                };

                SendOptions sendOptions = new SendOptions
                {
                    Reliability = true
                };

                PhotonNetwork.RaiseEvent(customManualInstantiationEventCode, data, raiseEventOptions, sendOptions);
            }
            else
            {
                Debug.LogError("Failed to allocate a ViewId.");

                Destroy(player);
            }
        }
        
        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == customManualInstantiationEventCode)
            {
                object[] data = (object[]) photonEvent.CustomData;

                GameObject player = Instantiate(networkedPlayerPrefab.gameObject, (Vector3) data[0], (Quaternion) data[1]);
                PhotonView photonView = player.GetComponent<PhotonView>();
                photonView.ViewID = (int) data[2];
            }
        }
    }
}
