using ExitGames.Client.Photon;
using Features.GlobalReferences;
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
        [SerializeField] private GridRuntimeDictionary_SO gridRuntimeDictionary;
        
        public Vector3 GridPosition { get; set; }

        private float _movementSpeed = 3f;
        private PhotonView _photonView;

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
        }
    
        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == RaiseEventCode.OnMasterChangeUnitGridPosition)
            {
                object[] data = (object[]) photonEvent.CustomData;
                int viewID = (int) data[0];
                if (_photonView.ViewID != viewID) return;
                
                MoveGameObjectToTarget(gameObject, (Vector3) data[1], (Vector3) data[2]);
            }
            else if (photonEvent.Code == RaiseEventCode.OnRequestChangeUnitGridPosition)
            {
                object[] data = (object[]) photonEvent.CustomData;
                int viewID = (int) data[0];
                if (_photonView.ViewID != viewID) return;
                
                gridRuntimeDictionary.TryGetValue((Vector3) data[1], out TileBehaviour targetTileBehaviour);
                if (targetTileBehaviour.ContainsUnit) return;
                
                NetworkMove(RaiseEventCode.OnMasterChangeUnitGridPosition, ReceiverGroup.All, _photonView.ViewID, (Vector3) data[1], (Vector3) data[2]);
            }
        }
    
        public void NetworkMove(byte eventCode, ReceiverGroup receiverGroup, int viewID, Vector3 targetGridPosition, Vector3 previousGridPosition)
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

        private void MoveGameObjectToTarget(GameObject movable, Vector3 newGridPosition, Vector3 previousGridPosition)
        {
            gridRuntimeDictionary.TryGetValue(newGridPosition, out TileBehaviour targetTileBehaviour);
            if (targetTileBehaviour.ContainsUnit) return;
            
            Vector3 targetPosition = targetTileBehaviour.transform.position;
            float time = Vector3.Distance(movable.transform.position, targetPosition) / _movementSpeed;
            LeanTween.move(movable, targetPosition, time);

            targetTileBehaviour.AddUnit(this);
            gridRuntimeDictionary.TryGetValue(previousGridPosition, out TileBehaviour previousTileBehaviour);
            if (previousTileBehaviour.ContainsUnit)
            {
                previousTileBehaviour.RemoveUnit();
            }
        }
    }
}
