using ExitGames.Client.Photon;
using Features.Experimental;
using Features.Unit.Modding;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Features.Unit
{
    public class UnitSpawner : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        [SerializeField] private LocalUnitBehaviour localPlayerPrefab;
        [SerializeField] private NetworkedUnitBehaviour networkedPlayerPrefab;

        public void Spawn()
        {
            SpawnPlayer();
        }

        public override void OnEnable()
        {
            base.OnEnable();
            TestingGenerator.onSpawnUnit += SpawnPlayer;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            TestingGenerator.onSpawnUnit -= SpawnPlayer;
        }

        private LocalUnitBehaviour SpawnPlayer()
        {
            GameObject player = Instantiate(localPlayerPrefab.gameObject, transform);
            player.transform.position = new Vector3(Random.Range(0, 5), Random.Range(0, 5), 0);
            PhotonView instantiationPhotonView = player.GetComponent<PhotonView>();

            if (PhotonNetwork.AllocateViewID(instantiationPhotonView))
            {
                object[] data = new object[]
                {
                    player.transform.position, player.transform.rotation, instantiationPhotonView.ViewID
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

                PhotonNetwork.RaiseEvent(RaiseEventCode.UnitManualInstantiation, data, raiseEventOptions, sendOptions);
            }
            else
            {
                Debug.LogError("Failed to allocate a ViewId.");

                Destroy(player);
            }

            return player.GetComponent<LocalUnitBehaviour>();
        }
        
        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == RaiseEventCode.UnitManualInstantiation)
            {
                object[] data = (object[]) photonEvent.CustomData;

                GameObject player = Instantiate(networkedPlayerPrefab.gameObject, (Vector3) data[0], (Quaternion) data[1]);
                PhotonView instantiationPhotonView = player.GetComponent<PhotonView>();
                instantiationPhotonView.ViewID = (int) data[2];
            }
        }
    }
}
