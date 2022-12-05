using ExitGames.Client.Photon;
using Features.Tiles;
using Features.Unit.Modding;
using Photon.Pun;
using Photon.Realtime;
using ThirdParty.LeanTween.Framework;
using UnityEngine;

namespace Features.Unit.GridMovement
{
    [RequireComponent(typeof(PhotonView), typeof(NetworkedUnitBehaviour))]
    public class NetworkedUnitTilePlacementBehaviour : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        [SerializeField] protected TileRuntimeDictionary_SO tileRuntimeDictionary;
        
        public Vector3Int GridPosition { get; set; }

        private float _movementSpeed = 3f;
        private PhotonView _photonView;

        //TODO: implement instantiating
        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
        }
    
        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == (int)RaiseEventCode.OnMasterChangeUnitGridPosition)
            {
                object[] data = (object[]) photonEvent.CustomData;
                int viewID = (int) data[0];
                if (_photonView.ViewID != viewID) return;
                
                MoveGameObjectToTarget(gameObject, (Vector3Int) data[1], (Vector3Int) data[2]);
            }
            else if (photonEvent.Code == (int)RaiseEventCode.OnRequestChangeUnitGridPosition)
            {
                object[] data = (object[]) photonEvent.CustomData;
                int viewID = (int) data[0];
                if (_photonView.ViewID != viewID) return;
                
                tileRuntimeDictionary.TryGetValue((Vector3Int) data[1], out TileContainer targetTileBehaviour);
                if (targetTileBehaviour.ContainsUnit) return;
                
                NetworkMove((int)RaiseEventCode.OnMasterChangeUnitGridPosition, ReceiverGroup.All, _photonView.ViewID, (Vector3Int) data[1], (Vector3Int) data[2]);
            }
        }
    
        public void NetworkMove(byte eventCode, ReceiverGroup receiverGroup, int viewID, Vector3Int targetGridPosition, Vector3Int previousGridPosition)
        {
            object[] data = new object[]
            {
                viewID,
                targetGridPosition,
                previousGridPosition
            };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = receiverGroup,
                CachingOption = EventCaching.AddToRoomCache
            };

            SendOptions sendOptions = new SendOptions
            {
                Reliability = true
            };

            PhotonNetwork.RaiseEvent(eventCode, data, raiseEventOptions, sendOptions);
        }

        private void MoveGameObjectToTarget(GameObject movable, Vector3Int newGridPosition, Vector3Int previousGridPosition)
        {
            tileRuntimeDictionary.TryGetValue(newGridPosition, out TileContainer targetTileContainer);
            if (targetTileContainer.ContainsUnit) return;
            
            Vector3 targetPosition = tileRuntimeDictionary.GetCellToWorldPosition(newGridPosition);
            float time = Vector3.Distance(movable.transform.position, targetPosition) / _movementSpeed;
            LeanTween.move(movable, targetPosition, time);

            targetTileContainer.AddUnit(this);

            if (!tileRuntimeDictionary.TryGetValue(previousGridPosition, out TileContainer previousTileBehaviour)) return;
            if (previousTileBehaviour.ContainsUnit)
            {
                previousTileBehaviour.RemoveUnit();
            }
        }
    }
}
