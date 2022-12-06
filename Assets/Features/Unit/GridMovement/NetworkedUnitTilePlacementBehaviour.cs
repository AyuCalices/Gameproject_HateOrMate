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

        public Vector3Int GridPosition => tileRuntimeDictionary.GetWorldToCellPosition(transform.position);
        
        private float _movementSpeed = 3f;
        private PhotonView _photonView;

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
                
                tileRuntimeDictionary.TryGetContent((Vector3Int) data[1], out RuntimeTile targetTileBehaviour);
                if (targetTileBehaviour.ContainsUnit) return;
                
                NetworkMove((int)RaiseEventCode.OnMasterChangeUnitGridPosition, ReceiverGroup.All, _photonView.ViewID, (Vector3Int) data[1], (Vector3Int) data[2]);
            }
        }

        public void RequestMove(Vector3Int targetTileGridPosition)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                NetworkMove((int)RaiseEventCode.OnMasterChangeUnitGridPosition, ReceiverGroup.All, _photonView.ViewID, targetTileGridPosition, GridPosition);
            }
            else
            {
                NetworkMove((int)RaiseEventCode.OnRequestChangeUnitGridPosition, ReceiverGroup.MasterClient, _photonView.ViewID, targetTileGridPosition, GridPosition);
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
            tileRuntimeDictionary.TryGetContent(newGridPosition, out RuntimeTile targetTileContainer);
            if (targetTileContainer.ContainsUnit) return;

            Vector3 targetPosition = tileRuntimeDictionary.GetCellToWorldPosition(newGridPosition);
            float time = Vector3.Distance(movable.transform.position, targetPosition) / _movementSpeed;
            LeanTween.move(movable, targetPosition, time);

            targetTileContainer.AddUnit(gameObject);
            
            if (!tileRuntimeDictionary.TryGetContent(previousGridPosition, out RuntimeTile previousTileBehaviour)) return;
            if (previousTileBehaviour.ContainsUnit)
            {
                previousTileBehaviour.RemoveUnit();
            }
        }
    }
}
