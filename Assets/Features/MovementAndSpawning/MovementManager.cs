using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Features.Battle.Scripts;
using Features.Connection;
using Features.Connection.Scripts.Utils;
using Features.Tiles.Scripts;
using Features.Unit.Scripts.Behaviours.Battle;
using Photon.Pun;
using Photon.Realtime;
using ThirdParty.LeanTween.Framework;
using UnityEngine;

namespace Features.MovementAndSpawning
{
    /// <summary>
    /// In order to prevent, that two Players can place a Unit at the same GridPosition at the same
    /// time, locking tile positions is handled by the MasterClient. When a Non-MasterClient wants
    /// to swap a tile position, he always needs to send a request to the MasterClient by a RaiseEvent.
    /// Inside the Callback, a new RaiseEvent will be called to update Positions if valid.
    ///
    /// On the other hand, if a MasterClient wants to change the Position, it directly calls a RaiseEvent
    /// to update Positions if valid.
    ///
    /// Methods starting with 'Perform' represents those position swaps, and 'Requests' are RaiseEvents
    /// that should call 'Perform'
    /// </summary>
    public class MovementManager : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        [Header("References")]
        [SerializeField] protected BattleData_SO battleData;

        public override void OnEnable()
        {
            base.OnEnable();

            MovementState.onPerformMovement += RequestGridStep;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            
            MovementState.onPerformMovement -= RequestGridStep;
        }

        private void RequestGridStep(NetworkedBattleBehaviour battleBehaviour, Vector3Int targetTileGridPosition, int skipLastMovementsCount)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PerformGridStep(battleBehaviour, targetTileGridPosition, skipLastMovementsCount, battleBehaviour.MovementSpeed);
            }
            else
            {
                RequestGridStep_RaiseEvent(battleBehaviour.NetworkedStatsBehaviour.PhotonView, targetTileGridPosition, skipLastMovementsCount, battleBehaviour.MovementSpeed);
            }
        }
        
        #region MasterClient Perform Methods

        /// <summary>
        /// Master Cast
        /// </summary>
        /// <param name="battleBehaviour"></param>
        /// <param name="targetTileGridPosition"></param>
        /// <param name="skipLastMovementsCount"></param>
        /// <param name="movementSpeed"></param>
        private void PerformGridStep(NetworkedBattleBehaviour battleBehaviour, Vector3Int targetTileGridPosition, int skipLastMovementsCount, float movementSpeed)
        {
            Vector3Int currentCellPosition = GridPositionHelper.GetCurrentCellPosition(battleData, battleBehaviour.transform);
            if (!TryGetNextPathfindingPosition(targetTileGridPosition, currentCellPosition, skipLastMovementsCount,
                out Vector3Int nextCellPosition))
            {
                EnterIdleState_RaiseEvent(battleBehaviour);
                return;
            }
                
            GridPositionHelper.UpdateUnitOnRuntimeTiles(battleData, battleBehaviour, currentCellPosition, nextCellPosition);
            PerformGridStep_RaiseEvent(battleBehaviour, nextCellPosition, movementSpeed);
        }
        
        #endregion

        #region RaiseEvents: Requests for MasterClient

        private void RequestGridStep_RaiseEvent(PhotonView photonView, Vector3Int targetCellPosition, int skipLastMovementsCount, float movementSpeed)
        {
            object[] data = new object[]
            {
                photonView.ViewID,
                targetCellPosition,
                skipLastMovementsCount,
                movementSpeed
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

            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnRequestGridStep, data, raiseEventOptions, sendOptions);
        }

        #endregion

        #region RaiseEvents: MasterClient sends result to all

        private void PerformGridStep_RaiseEvent(NetworkedBattleBehaviour battleBehaviour, Vector3Int nextCellPosition, float movementSpeed)
        {
            object[] data = new object[]
            {
                battleBehaviour.PhotonView.ViewID,
                nextCellPosition,
                movementSpeed
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

            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnPerformGridStep, data, raiseEventOptions, sendOptions);
        }

        private void EnterIdleState_RaiseEvent(NetworkedBattleBehaviour battleBehaviour)
        {
            object[] data = new object[]
            {
                battleBehaviour.PhotonView.ViewID,
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

            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnEnterUnitIdleState, data, raiseEventOptions, sendOptions);
        }
        
        #endregion

        #region Helper Functions

        private bool TryGetNextPathfindingPosition(Vector3Int targetCellPosition, Vector3Int startCellPosition, int skipLastMovementsCount, out Vector3Int nextCellPosition)
        {
            if (battleData.TileRuntimeDictionary.GenerateAStarPath(startCellPosition,
                targetCellPosition, out List<Vector3Int> path) && path.Count > skipLastMovementsCount)
            {
                nextCellPosition = path[0];
                return true;
            }

            nextCellPosition = default;
            return false;
        }

        private void MoveGameObjectToTarget(NetworkedBattleBehaviour battleBehaviour, Vector3Int nextCellPosition, float movementSpeed, Action onComplete)
        {
            Vector3 targetPosition = battleData.TileRuntimeDictionary.GetCellToWorldPosition(nextCellPosition);
            float time = Vector3.Distance(battleBehaviour.transform.position, targetPosition) / movementSpeed;
            LeanTween.move(battleBehaviour.gameObject, targetPosition, time).setOnComplete(onComplete.Invoke);
        }

        #endregion

        #region RaiseEvent Callbacks
        
        public void OnEvent(EventData photonEvent)
        {
            switch (photonEvent.Code)
            {
                case (int)RaiseEventCode.OnPerformGridStep:
                {
                    object[] data = (object[]) photonEvent.CustomData;
                    int viewID = (int) data[0];
                    if (battleData.AllUnitsRuntimeSet.GetUnitByViewID(viewID, out NetworkedBattleBehaviour networkedUnitBehaviour))
                    {
                        Vector3Int nextCellPosition = (Vector3Int) data[1];
                        float movementSpeed = (float) data[2];

                        //the masterClient already locked the position to prevent race conditions
                        if (!PhotonNetwork.IsMasterClient)
                        {
                            Vector3Int currentCellPosition = GridPositionHelper.GetCurrentCellPosition(battleData, networkedUnitBehaviour.transform);
                            GridPositionHelper.UpdateUnitOnRuntimeTiles(battleData, networkedUnitBehaviour, currentCellPosition, nextCellPosition);
                        }
                    
                        MoveGameObjectToTarget(networkedUnitBehaviour, nextCellPosition, movementSpeed, () =>
                        {
                            if (!networkedUnitBehaviour.TryRequestAttackState() || !networkedUnitBehaviour.TryRequestMovementStateByClosestUnit() || networkedUnitBehaviour.CurrentState is not DeathState)
                            {
                                networkedUnitBehaviour.ForceIdleState();
                            }
                        });
                    }

                    break;
                }
                case (int)RaiseEventCode.OnRequestGridStep:
                {
                    object[] data = (object[]) photonEvent.CustomData;
                    int viewID = (int) data[0];
                    if (battleData.AllUnitsRuntimeSet.GetUnitByViewID(viewID, out NetworkedBattleBehaviour networkedUnitBehaviour))
                    {
                        Vector3Int targetCellPosition = (Vector3Int) data[1];
                        int skipLastMovementsCount = (int) data[2];
                        float movementSpeed = (float) data[3];

                        PerformGridStep(networkedUnitBehaviour, targetCellPosition, skipLastMovementsCount, movementSpeed);
                    }

                    break;
                }
                case (int)RaiseEventCode.OnEnterUnitIdleState:
                {
                    object[] data = (object[]) photonEvent.CustomData;
                    int viewID = (int) data[0];

                    if (battleData.AllUnitsRuntimeSet.GetUnitByViewID(viewID,
                        out NetworkedBattleBehaviour networkedUnitBehaviour) && networkedUnitBehaviour.TeamTagTypes.Contains(TeamTagType.Own))
                    {
                        networkedUnitBehaviour.ForceIdleState();
                    }

                    break;
                }
            }
        }
        
        #endregion
    }
}
