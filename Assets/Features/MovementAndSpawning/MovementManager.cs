using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Features.Battle.Scripts;
using Features.Connection;
using Features.Connection.Scripts.Utils;
using Features.Tiles.Scripts;
using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Stats;
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

        private void RequestGridStep(UnitServiceProvider unitServiceProvider, Vector3Int targetTileGridPosition, int skipLastMovementsCount)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PerformGridStep(unitServiceProvider, targetTileGridPosition, skipLastMovementsCount, 
                    unitServiceProvider.GetService<NetworkedStatsBehaviour>().GetFinalStat(StatType.MovementSpeed));
            }
            else
            {
                RequestGridStep_RaiseEvent(unitServiceProvider.GetService<PhotonView>(), targetTileGridPosition, skipLastMovementsCount, 
                    unitServiceProvider.GetService<NetworkedStatsBehaviour>().GetFinalStat(StatType.MovementSpeed));
            }
        }
        
        #region MasterClient Perform Methods

        /// <summary>
        /// Master Cast
        /// </summary>
        /// <param name="unitServiceProvider"></param>
        /// <param name="targetTileGridPosition"></param>
        /// <param name="skipLastMovementsCount"></param>
        /// <param name="movementSpeed"></param>
        private void PerformGridStep(UnitServiceProvider unitServiceProvider, Vector3Int targetTileGridPosition, int skipLastMovementsCount, float movementSpeed)
        {
            Vector3Int currentCellPosition = GridPositionHelper.GetCurrentCellPosition(battleData, unitServiceProvider.transform);
            if (!TryGetNextPathfindingPosition(targetTileGridPosition, currentCellPosition, skipLastMovementsCount,
                out Vector3Int nextCellPosition))
            {
                EnterIdleState_RaiseEvent(unitServiceProvider.GetService<PhotonView>());
                return;
            }
                
            GridPositionHelper.UpdateUnitOnRuntimeTiles(battleData, unitServiceProvider, currentCellPosition, nextCellPosition);
            PerformGridStep_RaiseEvent(unitServiceProvider.GetService<PhotonView>(), nextCellPosition, movementSpeed);
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

        private void PerformGridStep_RaiseEvent(PhotonView unitPhotonView, Vector3Int nextCellPosition, float movementSpeed)
        {
            object[] data = new object[]
            {
                unitPhotonView.ViewID,
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

        private void EnterIdleState_RaiseEvent(PhotonView unitPhotonView)
        {
            object[] data = new object[]
            {
                unitPhotonView.ViewID,
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

        private void MoveGameObjectToTarget(UnitServiceProvider battleBehaviour, Vector3Int nextCellPosition, float movementSpeed, Action onComplete)
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
                    if (battleData.AllUnitsRuntimeSet.GetUnitByViewID(viewID, out UnitServiceProvider unitServiceProvider))
                    {
                        Vector3Int nextCellPosition = (Vector3Int) data[1];
                        float movementSpeed = (float) data[2];

                        //the masterClient already locked the position to prevent race conditions
                        if (!PhotonNetwork.IsMasterClient)
                        {
                            Vector3Int currentCellPosition = GridPositionHelper.GetCurrentCellPosition(battleData, unitServiceProvider.transform);
                            GridPositionHelper.UpdateUnitOnRuntimeTiles(battleData, unitServiceProvider, currentCellPosition, nextCellPosition);
                        }
                    
                        MoveGameObjectToTarget(unitServiceProvider, nextCellPosition, movementSpeed, () =>
                        {
                            NetworkedBattleBehaviour unitBattleBehaviour = unitServiceProvider.GetService<NetworkedBattleBehaviour>();
                            if (!unitBattleBehaviour.TryRequestAttackState() || !unitBattleBehaviour.TryRequestMovementStateByClosestUnit() || unitBattleBehaviour.CurrentState is not DeathState)
                            {
                                unitBattleBehaviour.ForceIdleState();
                            }
                        });
                    }

                    break;
                }
                case (int)RaiseEventCode.OnRequestGridStep:
                {
                    object[] data = (object[]) photonEvent.CustomData;
                    int viewID = (int) data[0];
                    if (battleData.AllUnitsRuntimeSet.GetUnitByViewID(viewID, out UnitServiceProvider unitServiceProvider))
                    {
                        Vector3Int targetCellPosition = (Vector3Int) data[1];
                        int skipLastMovementsCount = (int) data[2];
                        float movementSpeed = (float) data[3];

                        PerformGridStep(unitServiceProvider, targetCellPosition, skipLastMovementsCount, movementSpeed);
                    }

                    break;
                }
                case (int)RaiseEventCode.OnEnterUnitIdleState:
                {
                    object[] data = (object[]) photonEvent.CustomData;
                    int viewID = (int) data[0];

                    if (battleData.AllUnitsRuntimeSet.GetUnitByViewID(viewID, out UnitServiceProvider unitServiceProvider) 
                        && unitServiceProvider.TeamTagTypes.Contains(TeamTagType.Own))
                    {
                        unitServiceProvider.GetService<NetworkedBattleBehaviour>().ForceIdleState();
                    }

                    break;
                }
            }
        }
        
        #endregion
    }
}
