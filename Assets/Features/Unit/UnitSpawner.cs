using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Features.Experimental;
using Features.GlobalReferences;
using Features.Unit.GridMovement;
using Features.Unit.Modding;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Features.Unit
{
    public class UnitSpawner : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        [SerializeField] private GridRuntimeDictionary_SO gridRuntimeDictionary;
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
            LocalUnitBehaviour player = Instantiate(localPlayerPrefab, transform);
            
            int randomElement = Random.Range(0, gridRuntimeDictionary.GetItems().Count);
            KeyValuePair<Vector2, TileBehaviour> keyValuePair = gridRuntimeDictionary.GetItems().ElementAt(randomElement);
            player.transform.position = keyValuePair.Value.transform.position;
            
            PhotonView instantiationPhotonView = player.GetComponent<PhotonView>();

            if (PhotonNetwork.AllocateViewID(instantiationPhotonView))
            {
                var playerTransform = player.transform;
                object[] data = new object[]
                {
                    playerTransform.position, playerTransform.rotation, instantiationPhotonView.ViewID, keyValuePair.Key
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

                PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnUnitManualInstantiation, data, raiseEventOptions, sendOptions);
                player.OnPhotonViewIdAllocated();
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
            if (photonEvent.Code == (int)RaiseEventCode.OnUnitManualInstantiation)
            {
                object[] data = (object[]) photonEvent.CustomData;

                NetworkedUnitBehaviour player = Instantiate(networkedPlayerPrefab, (Vector3) data[0], (Quaternion) data[1]);
                PhotonView instantiationPhotonView = player.GetComponent<PhotonView>();
                instantiationPhotonView.ViewID = (int) data[2];

                //TODO: this is not perfect
                if (gridRuntimeDictionary.GetItems().TryGetValue((Vector2) data[3], out TileBehaviour tile))
                {
                    if (!tile.ContainsUnit)
                    {
                        tile.AddUnit(player.GetComponent<NetworkedUnitTilePlacementBehaviour>());
                        player.GetComponent<NetworkedUnitTilePlacementBehaviour>().GridPosition = (Vector2) data[3];
                    }
                }
                
                player.OnPhotonViewIdAllocated();
            }
        }
    }
}
