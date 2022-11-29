using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Features.Unit
{
    [RequireComponent(typeof(PhotonView))]
    public class NetworkedUnitDragBehaviour : MonoBehaviourPunCallbacks, IOnEventCallback
    {
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
            Debug.Log(photonEvent.Code);
            if (photonEvent.Code == masterManualInstantiationEventCode)
            {
                object[] data = (object[]) photonEvent.CustomData;
                int viewID = (int) data[0];
                if (_photonView.ViewID != viewID) return;
                
                Vector3 targetPosition = (Vector3) data[1];
                MoveGameObjectToTarget(gameObject, targetPosition);
            }
            else if (photonEvent.Code == requestManualInstantiationEventCode)
            {
                object[] data = (object[]) photonEvent.CustomData;
                int viewID = (int) data[0];
                if (_photonView.ViewID != viewID) return;
                
                NetworkMove(_photonView.ViewID, (Vector3) data[1]);
            }
        }
    
        public void NetworkMove(int viewID, Vector3 targetPosition)
        {
            object[] data = new object[]
            {
                viewID,
                targetPosition
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

        public void RequestMove(int viewID, Vector3 targetPosition)
        {
            object[] data = new object[]
            {
                viewID,
                targetPosition
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
        
        private void MoveGameObjectToTarget(GameObject movable, Vector3 target)
        {
            float time = Vector3.Distance(movable.transform.position, target) / _movementSpeed;
            LeanTween.move(movable, target, time);
        }
    }
}
