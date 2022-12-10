using System;
using System.Collections.Generic;
using DataStructures.StateLogic;
using ExitGames.Client.Photon;
using Features.Tiles;
using Features.Unit.GridMovement;
using Features.Unit.Modding;
using Photon.Pun;
using Photon.Realtime;
using ThirdParty.LeanTween.Framework;
using UnityEngine;

namespace Features.Unit.Battle.Scripts
{
    public class MovementState : IState
    {
        private readonly BattleBehaviour _battleBehaviour;
        private readonly TileRuntimeDictionary_SO _tileRuntimeDictionary;
        private readonly float _movementSpeed;
        private readonly Vector3Int _targetPosition;
        private readonly int _skipLastMovementsCount;

        private Vector3Int CurrentCellPosition => _tileRuntimeDictionary.GetWorldToCellPosition(_battleBehaviour.transform.position);

        public MovementState(BattleBehaviour battleBehaviour, TileRuntimeDictionary_SO tileRuntimeDictionary, float movementSpeed, Vector3Int targetPosition, int skipLastMovementsCount)
        {
            _battleBehaviour = battleBehaviour;
            _tileRuntimeDictionary = tileRuntimeDictionary;
            _movementSpeed = movementSpeed;
            _targetPosition = targetPosition;
            _skipLastMovementsCount = skipLastMovementsCount;
        }

        public void Enter()
        {
            Debug.Log("Enter");
            RequestMove(_targetPosition, _skipLastMovementsCount);
        }

        public void Execute()
        {
        }

        public void Exit()
        {  
        }
        
        public void OnEvent(EventData photonEvent)
        {
            //moves unit to target position for all players
            if (photonEvent.Code == (int)RaiseEventCode.OnPerformGridPositionSwap)
            {
                object[] data = (object[]) photonEvent.CustomData;
                int viewID = (int) data[0];
                if (_battleBehaviour.photonView.ViewID != viewID) return;

                Vector3Int nextCellPosition = (Vector3Int) data[1];
                MoveGameObjectToTarget(_battleBehaviour.gameObject, nextCellPosition, () =>
                {
                    if (!_battleBehaviour.TryRequestAttackState() || !_battleBehaviour.TryRequestMovementStateByClosestUnit() || _battleBehaviour.CurrentState is not DeathState)
                    {
                        _battleBehaviour.ForceIdleState();
                    }
                });
            }
            
            //master calculated path
            if (photonEvent.Code == (int)RaiseEventCode.OnRequestMoveToTarget)
            {
                object[] data = (object[]) photonEvent.CustomData;
                int viewID = (int) data[0];
                if (_battleBehaviour.photonView.ViewID != viewID) return;

                Vector3Int targetCellPosition = (Vector3Int) data[1];
                Vector3Int currentCellPosition = (Vector3Int) data[2];
                int skipLastMovementsCount = (int) data[3];
                OnMasterChangeUnitGridPosition(targetCellPosition, currentCellPosition, skipLastMovementsCount);
            }
        }

        private void RequestMove(Vector3Int targetTileGridPosition, int skipLastMovementsCount)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                OnMasterChangeUnitGridPosition(targetTileGridPosition, CurrentCellPosition, skipLastMovementsCount);
            }
            else
            {
                RequestMoveToTarget(targetTileGridPosition, CurrentCellPosition, skipLastMovementsCount);
            }
        }
        
        private void OnMasterChangeUnitGridPosition(Vector3Int targetCellPosition, Vector3Int currentCellPosition, int skipLastMovementsCount)
        {
            if (!TryGetNextPosition(targetCellPosition, currentCellPosition, skipLastMovementsCount, out Vector3Int nextCellPosition)) return;

            UpdateUnitOnRuntimeTiles(currentCellPosition, nextCellPosition);

            object[] data = new object[]
            {
                _battleBehaviour.photonView.ViewID,
                nextCellPosition
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
        
        private bool TryGetNextPosition(Vector3Int targetCellPosition, Vector3Int startCellPosition, int skipLastMovementsCount, out Vector3Int nextCellPosition)
        {
            if (_tileRuntimeDictionary.GenerateAStarPath(startCellPosition,
                targetCellPosition, out List<Vector3Int> path) && path.Count > skipLastMovementsCount)
            {
                nextCellPosition = path[0];
                return true;
            }

            nextCellPosition = default;
            return false;
        }
        
        private void UpdateUnitOnRuntimeTiles(Vector3Int currentCellPosition, Vector3Int nextCellPosition)
        {
            RuntimeTile targetTileContainer = _tileRuntimeDictionary.GetContent(nextCellPosition);
            targetTileContainer.AddUnit(_battleBehaviour.gameObject);

            if (_tileRuntimeDictionary.TryGetContent(currentCellPosition, out RuntimeTile previousTileBehaviour))
            {
                previousTileBehaviour.RemoveUnit();
            }
        }
        
        private void RequestMoveToTarget(Vector3Int targetCellPosition, Vector3Int currentCellPosition, int skipLastMovementsCount)
        {
            object[] data = new object[]
            {
                _battleBehaviour.photonView.ViewID,
                targetCellPosition,
                currentCellPosition,
                skipLastMovementsCount
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
            Vector3 targetPosition = _tileRuntimeDictionary.GetCellToWorldPosition(nextCellPosition);
            float time = Vector3.Distance(movable.transform.position, targetPosition) / _movementSpeed;
            LeanTween.move(movable, targetPosition, time).setOnComplete(onComplete.Invoke);
        }
    }
}
