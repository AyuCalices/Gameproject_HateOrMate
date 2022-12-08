using System;
using System.Collections.Generic;
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

        public Vector3Int CurrentCellPosition => tileRuntimeDictionary.GetWorldToCellPosition(transform.position);
        
        protected float _movementSpeed = 3f;
        private PhotonView _photonView;

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
        }
    
        public void OnEvent(EventData photonEvent)
        {
            //moves unit to target position for all players
            if (photonEvent.Code == (int)RaiseEventCode.OnPerformGridPositionSwap)
            {
                object[] data = (object[]) photonEvent.CustomData;
                int viewID = (int) data[0];
                if (_photonView.ViewID != viewID) return;
                
                Vector3Int targetCellPosition = (Vector3Int) data[1];
                Vector3Int nextCellPosition = (Vector3Int) data[2];
                int pathMinLength = (int) data[3];
                MoveGameObjectToTarget(gameObject, nextCellPosition, () =>
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        OnMasterChangeUnitGridPosition(targetCellPosition, nextCellPosition, pathMinLength);
                    }
                });
            }
            
            //master calculated path
            if (photonEvent.Code == (int)RaiseEventCode.OnRequestMoveToTarget)
            {
                object[] data = (object[]) photonEvent.CustomData;
                int viewID = (int) data[0];
                if (_photonView.ViewID != viewID) return;

                Vector3Int targetCellPosition = (Vector3Int) data[1];
                Vector3Int currentCellPosition = (Vector3Int) data[2];
                int pathMinLength = (int) data[3];
                OnMasterChangeUnitGridPosition(targetCellPosition, currentCellPosition, pathMinLength);
            }
        }

        public void RequestMove(Vector3Int targetTileGridPosition, int pathMinLength)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                OnMasterChangeUnitGridPosition(targetTileGridPosition, CurrentCellPosition, pathMinLength);
            }
            else
            {
                RequestMoveToTarget(targetTileGridPosition, CurrentCellPosition, pathMinLength);
            }
        }

        private bool TryGetNextPosition(Vector3Int targetCellPosition, Vector3Int startCellPosition, int pathMinLength, out Vector3Int nextCellPosition)
        {
            if (tileRuntimeDictionary.GenerateAStarPath(startCellPosition,
                targetCellPosition, out List<Vector3Int> path) && path.Count >= pathMinLength)
            {
                nextCellPosition = path[0];
                return true;
            }

            nextCellPosition = default;
            return false;
        }

        private void OnMasterChangeUnitGridPosition(Vector3Int targetCellPosition, Vector3Int currentCellPosition, int pathMinLength)
        {
            if (!TryGetNextPosition(targetCellPosition, currentCellPosition, pathMinLength, out Vector3Int nextCellPosition)) return;

            RuntimeTile targetTileContainer = tileRuntimeDictionary.GetContent(nextCellPosition);
            targetTileContainer.AddUnit(gameObject);

            if (tileRuntimeDictionary.TryGetContent(currentCellPosition, out RuntimeTile previousTileBehaviour))
            {
                if (previousTileBehaviour.ContainsUnit)
                {
                    previousTileBehaviour.RemoveUnit();
                }
            }
            
            object[] data = new object[]
            {
                _photonView.ViewID,
                targetCellPosition,
                nextCellPosition,
                pathMinLength
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

            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnPerformGridPositionSwap, data, raiseEventOptions, sendOptions);
        }

        private void RequestMoveToTarget(Vector3Int targetCellPosition, Vector3Int currentCellPosition, int pathMinLength)
        {
            object[] data = new object[]
            {
                _photonView.ViewID,
                targetCellPosition,
                currentCellPosition,
                pathMinLength
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

            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnRequestMoveToTarget, data, raiseEventOptions, sendOptions);
        }

        private void MoveGameObjectToTarget(GameObject movable, Vector3Int nextCellPosition, Action onComplete)
        {
            Vector3 targetPosition = tileRuntimeDictionary.GetCellToWorldPosition(nextCellPosition);
            float time = Vector3.Distance(movable.transform.position, targetPosition) / _movementSpeed;
            LeanTween.move(movable, targetPosition, time).setOnComplete(onComplete.Invoke);
        }
    }
}
