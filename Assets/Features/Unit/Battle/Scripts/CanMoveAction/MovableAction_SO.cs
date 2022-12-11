using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Features.Tiles;
using Photon.Pun;
using Photon.Realtime;
using ThirdParty.LeanTween.Framework;
using UnityEngine;

namespace Features.Unit.Battle.Scripts.CanMoveAction
{
    [CreateAssetMenu]
    public class MovableAction_SO : IsMovable_SO
    {
        [SerializeField] private TileRuntimeDictionary_SO tileRuntimeDictionary;
        [SerializeField] private float movementSpeed;
    
        private Vector3Int CurrentCellPosition(BattleBehaviour battleBehaviour) => tileRuntimeDictionary.GetWorldToCellPosition(battleBehaviour.transform.position);

        public override void RequestMove(BattleBehaviour battleBehaviour, Vector3Int targetTileGridPosition, int skipLastMovementsCount)
        {
            Vector3Int currentCellPosition = CurrentCellPosition(battleBehaviour);
            if (PhotonNetwork.IsMasterClient)
            {
                OnMasterChangeUnitGridPosition(battleBehaviour, targetTileGridPosition, currentCellPosition, skipLastMovementsCount);
            }
            else
            {
                RequestMoveToTarget(battleBehaviour.photonView, targetTileGridPosition, currentCellPosition, skipLastMovementsCount);
            }
        }
        
        public override void OnEvent(BattleBehaviour battleBehaviour, EventData photonEvent)
        {
            //moves unit to target position for all players
            if (photonEvent.Code == (int)RaiseEventCode.OnPerformGridPositionSwap)
            {
                object[] data = (object[]) photonEvent.CustomData;
                int viewID = (int) data[0];
                if (battleBehaviour.photonView.ViewID != viewID) return;

                Vector3Int nextCellPosition = (Vector3Int) data[1];
                MoveGameObjectToTarget(battleBehaviour.gameObject, nextCellPosition, () =>
                {
                    if (!battleBehaviour.TryRequestAttackState() || !battleBehaviour.TryRequestMovementStateByClosestUnit() || battleBehaviour.CurrentState is not DeathState)
                    {
                        battleBehaviour.ForceIdleState();
                    }
                });
            }
            
            //master calculated path
            if (photonEvent.Code == (int)RaiseEventCode.OnRequestMoveToTarget)
            {
                object[] data = (object[]) photonEvent.CustomData;
                int viewID = (int) data[0];
                if (battleBehaviour.photonView.ViewID != viewID) return;

                Vector3Int targetCellPosition = (Vector3Int) data[1];
                Vector3Int currentCellPosition = (Vector3Int) data[2];
                int skipLastMovementsCount = (int) data[3];
                OnMasterChangeUnitGridPosition(battleBehaviour, targetCellPosition, currentCellPosition, skipLastMovementsCount);
            }
        }

        private void OnMasterChangeUnitGridPosition(BattleBehaviour battleBehaviour, Vector3Int targetCellPosition, Vector3Int currentCellPosition, int skipLastMovementsCount)
        {
            if (!TryGetNextPosition(targetCellPosition, currentCellPosition, skipLastMovementsCount, out Vector3Int nextCellPosition)) return;

            UpdateUnitOnRuntimeTiles(battleBehaviour, currentCellPosition, nextCellPosition);

            object[] data = new object[]
            {
                battleBehaviour.photonView.ViewID,
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
            if (tileRuntimeDictionary.GenerateAStarPath(startCellPosition,
                targetCellPosition, out List<Vector3Int> path) && path.Count > skipLastMovementsCount)
            {
                nextCellPosition = path[0];
                return true;
            }

            nextCellPosition = default;
            return false;
        }
        
        private void UpdateUnitOnRuntimeTiles(BattleBehaviour battleBehaviour, Vector3Int currentCellPosition, Vector3Int nextCellPosition)
        {
            RuntimeTile targetTileContainer = tileRuntimeDictionary.GetContent(nextCellPosition);
            targetTileContainer.AddUnit(battleBehaviour.gameObject);

            if (tileRuntimeDictionary.TryGetContent(currentCellPosition, out RuntimeTile previousTileBehaviour))
            {
                previousTileBehaviour.RemoveUnit();
            }
        }
        
        private void RequestMoveToTarget(PhotonView photonView, Vector3Int targetCellPosition, Vector3Int currentCellPosition, int skipLastMovementsCount)
        {
            object[] data = new object[]
            {
                photonView.ViewID,
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
            Vector3 targetPosition = tileRuntimeDictionary.GetCellToWorldPosition(nextCellPosition);
            float time = Vector3.Distance(movable.transform.position, targetPosition) / movementSpeed;
            LeanTween.move(movable, targetPosition, time).setOnComplete(onComplete.Invoke);
        }
    }
}