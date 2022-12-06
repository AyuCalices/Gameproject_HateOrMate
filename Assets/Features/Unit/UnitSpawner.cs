using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Features.Experimental;
using Features.GlobalReferences;
using Features.Tiles;
using Features.Unit.GridMovement;
using Features.Unit.Modding;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Features.Unit
{
    public class UnitSpawner : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        [SerializeField] private TileRuntimeDictionary_SO tileRuntimeDictionary;
        [SerializeField] private GameObject localPlayerPrefab;
        [SerializeField] private GameObject networkedPlayerPrefab;
        

        public override void OnEnable()
        {
            base.OnEnable();
            TestingGenerator.onSpawnUnit += Spawn;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            TestingGenerator.onSpawnUnit -= Spawn;
        }

        public void Spawn()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                SpawnUnit(PhotonNetwork.IsMasterClient);
            }
            else
            {
                RequestSpawnRaiseEvent(PhotonNetwork.IsMasterClient);
            }
        }

        private void SpawnUnit(bool isSpawnedByMaster)
        {
            if (!tileRuntimeDictionary.TryGetRandomPlaceableTileBehaviour(
                out KeyValuePair<Vector3Int, RuntimeTile> tileKeyValuePair)) return;

            Vector3Int gridPosition = tileKeyValuePair.Key;
            RuntimeTile runtimeTile = tileKeyValuePair.Value;
            
            GameObject player = Instantiate(isSpawnedByMaster ? localPlayerPrefab : networkedPlayerPrefab, transform);
            runtimeTile.AddUnit(player.GetComponent<NetworkedUnitTilePlacementBehaviour>());
            player.transform.position = tileRuntimeDictionary.GetCellToWorldPosition(gridPosition);

            MasterSpawnRaiseEvent(player, gridPosition, isSpawnedByMaster);
        }

        private void MasterSpawnRaiseEvent(GameObject playerPrefab, Vector3Int gridPosition, bool isSpawnedByMaster)
        {
            //Raise Event
            PhotonView instantiationPhotonView = playerPrefab.GetComponent<PhotonView>();
            if (PhotonNetwork.AllocateViewID(instantiationPhotonView))
            {
                var playerTransform = playerPrefab.transform;
                object[] data = new object[]
                {
                    playerTransform.position, playerTransform.rotation, instantiationPhotonView.ViewID, gridPosition, isSpawnedByMaster
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
                playerPrefab.GetComponent<NetworkedUnitBehaviour>().OnPhotonViewIdAllocated();
            }
            else
            {
                Debug.LogError("Failed to allocate a ViewId.");

                Destroy(playerPrefab);
            }
        }
        
        private void RequestSpawnRaiseEvent(bool isSpawnedByMaster)
        {
            object[] data = new object[]
            {
                isSpawnedByMaster
            };
            
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.MasterClient,
                CachingOption = EventCaching.AddToRoomCache
            };

            SendOptions sendOptions = new SendOptions
            {
                Reliability = true
            };

            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnRequestUnitManualInstantiation, data, raiseEventOptions, sendOptions);
        }
        
        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == (int)RaiseEventCode.OnUnitManualInstantiation)
            {
                object[] data = (object[]) photonEvent.CustomData;

                //instantiate and prepare photon view
                GameObject player = Instantiate((bool) data[4] ? networkedPlayerPrefab : localPlayerPrefab, (Vector3) data[0], (Quaternion) data[1]);
                PhotonView instantiationPhotonView = player.GetComponent<PhotonView>();
                instantiationPhotonView.ViewID = (int) data[2];

                //sed grid position for all clients
                NetworkedUnitTilePlacementBehaviour networkedUnitTilePlacementBehaviour =
                    player.GetComponent<NetworkedUnitTilePlacementBehaviour>();
                if (tileRuntimeDictionary.TryGetByGridPosition((Vector3Int) data[3], out RuntimeTile tileBehaviour))
                {
                    tileBehaviour.AddUnit(networkedUnitTilePlacementBehaviour);
                }
                
                player.GetComponent<NetworkedUnitBehaviour>().OnPhotonViewIdAllocated();
            }

            if (photonEvent.Code == (int)RaiseEventCode.OnRequestUnitManualInstantiation)
            {
                object[] data = (object[]) photonEvent.CustomData;
                SpawnUnit((bool) data[0]);
            }
        }
    }
}
