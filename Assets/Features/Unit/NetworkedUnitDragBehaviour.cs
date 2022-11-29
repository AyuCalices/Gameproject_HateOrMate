using ExitGames.Client.Photon;
using Features.Grid;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Features.Unit
{
    [RequireComponent(typeof(PhotonView), typeof(NetworkedUnitBehaviour))]
    public class NetworkedUnitDragBehaviour : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        [SerializeField] private GridRuntimeDictionary_SO gridRuntimeDictionary;
        
        private readonly byte masterManualInstantiationEventCode = 56;
        private readonly byte requestManualInstantiationEventCode = 69;
    
        private float _movementSpeed = 3f;
        private PhotonView _photonView;

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
        }
    
        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == masterManualInstantiationEventCode)
            {
                object[] data = (object[]) photonEvent.CustomData;
                int viewID = (int) data[0];
                if (_photonView.ViewID != viewID) return;
                
                MoveGameObjectToTarget(gameObject, (Vector3) data[1], (Vector3) data[2]);
            }
            else if (photonEvent.Code == requestManualInstantiationEventCode)
            {
                object[] data = (object[]) photonEvent.CustomData;
                int viewID = (int) data[0];
                if (_photonView.ViewID != viewID) return;
                
                gridRuntimeDictionary.TryGetValue((Vector3) data[1], out Tile targetTile);
                if (targetTile.ContainsUnit) return;
                
                NetworkMove(_photonView.ViewID, (Vector3) data[1], (Vector3) data[2]);
            }
        }
    
        public void NetworkMove(int viewID, Vector3 targetGridPosition, Vector3 previousGridPosition)
        {
            object[] data = new object[]
            {
                viewID,
                targetGridPosition,
                previousGridPosition
            };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.All,
                CachingOption = EventCaching.AddToRoomCache
            };

            SendOptions sendOptions = new SendOptions
            {
                Reliability = true
            };

            PhotonNetwork.RaiseEvent(masterManualInstantiationEventCode, data, raiseEventOptions, sendOptions);
        }

        public void RequestMove(int viewID, Vector3 targetGridPosition, Vector3 previousGridPosition)
        {
            object[] data = new object[]
            {
                viewID,
                targetGridPosition,
                previousGridPosition
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

            PhotonNetwork.RaiseEvent(requestManualInstantiationEventCode, data, raiseEventOptions, sendOptions);
        }
        
        private void MoveGameObjectToTarget(GameObject movable, Vector3 newGridPosition, Vector3 previousGridPosition)
        {
            gridRuntimeDictionary.TryGetValue(newGridPosition, out Tile targetTile);
            if (targetTile.ContainsUnit) return;
            
            Vector3 targetPosition = targetTile.transform.position;
            float time = Vector3.Distance(movable.transform.position, targetPosition) / _movementSpeed;
            LeanTween.move(movable, targetPosition, time);

            targetTile.AddUnit(GetComponent<NetworkedUnitBehaviour>());
            gridRuntimeDictionary.TryGetValue(previousGridPosition, out Tile previousTile);
            if (previousTile.ContainsUnit)
            {
                previousTile.RemoveUnit();
            }
        }
    }
}
