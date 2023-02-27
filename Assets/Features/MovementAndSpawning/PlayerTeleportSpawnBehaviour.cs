using System.Collections.Generic;
using ExitGames.Client.Photon;
using Features.Battle.Scripts;
using Features.Connection;
using Features.Connection.Scripts.Utils;
using Features.Loot.Scripts.GeneratedLoot;
using Features.Tiles.Scripts;
using Features.Unit.Scripts;
using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Battle;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Features.MovementAndSpawning
{
    public class PlayerTeleportSpawnBehaviour : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        [SerializeField] private BattleData_SO battleData;
        
        public override void OnEnable()
        {
            base.OnEnable();
        
            UnitDragPlacementBehaviour.onPerformTeleport += RequestTeleport;
        }

        public override void OnDisable()
        {
            base.OnDisable();
        
            UnitDragPlacementBehaviour.onPerformTeleport -= RequestTeleport;
        }
        
        private void RequestTeleport(NetworkedBattleBehaviour battleBehaviour, Vector3Int targetTileGridPosition)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PerformGridTeleport(battleBehaviour, targetTileGridPosition);
            }
            else
            {
                RequestTeleport_RaiseEvent(battleBehaviour.NetworkedStatsBehaviour.PhotonView, targetTileGridPosition);
            }
        }

        #region MasterClient Perform Methods

        private void PerformGridTeleport(NetworkedBattleBehaviour battleBehaviour, Vector3Int targetCellPosition)
        {
            Vector3Int currentCellPosition = GridPositionHelper.GetCurrentCellPosition(battleData, battleBehaviour.transform);

            if (!GridPositionHelper.IsViablePosition(battleData, targetCellPosition)) return;
            
            GridPositionHelper.UpdateUnitOnRuntimeTiles(battleData, battleBehaviour, currentCellPosition, targetCellPosition);
            PerformGridTeleport_RaiseEvent(battleBehaviour, targetCellPosition);
        }

        #endregion

        #region RaiseEvents: MasterClient sends result to all

        private void PerformGridTeleport_RaiseEvent(NetworkedBattleBehaviour battleBehaviour, Vector3Int targetCellPosition)
        {
            object[] data = new object[]
            {
                battleBehaviour.PhotonView.ViewID,
                targetCellPosition
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

            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnPerformGridTeleport, data, raiseEventOptions, sendOptions);
        }
        
        #endregion
        
        #region RaiseEvents: Requests for MasterClient
    
        private void RequestTeleport_RaiseEvent(PhotonView photonView, Vector3Int targetCellPosition)
        {
            object[] data = new object[]
            {
                photonView.ViewID,
                targetCellPosition
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

            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnRequestTeleport, data, raiseEventOptions, sendOptions);
        }
    
        #endregion

        #region Helper Functions
        
        private void TeleportObjectToTarget(NetworkedBattleBehaviour battleBehaviour, Vector3Int nextCellPosition)
        {
            Vector3 targetPosition = battleData.TileRuntimeDictionary.GetCellToWorldPosition(nextCellPosition);
            battleBehaviour.transform.position = targetPosition;
        }
        
        #endregion
        
        #region RaiseEvent Callbacks
        
        public void OnEvent(EventData photonEvent)
        {
            switch (photonEvent.Code)
            {
                case (int)RaiseEventCode.OnPerformGridTeleport:
                {
                    object[] data = (object[]) photonEvent.CustomData;
                    int viewID = (int) data[0];
                    if (battleData.AllUnitsRuntimeSet.GetUnitByViewID(viewID, out NetworkedBattleBehaviour networkedUnitBehaviour))
                    {
                        Vector3Int nextCellPosition = (Vector3Int) data[1];

                        if (!PhotonNetwork.IsMasterClient)
                        {
                            Vector3Int currentCellPosition = GridPositionHelper.GetCurrentCellPosition(battleData, networkedUnitBehaviour.transform);
                            GridPositionHelper.UpdateUnitOnRuntimeTiles(battleData, networkedUnitBehaviour, currentCellPosition, nextCellPosition);
                        }
                        
                        if (networkedUnitBehaviour.CurrentState is BenchedState)
                        {
                            networkedUnitBehaviour.ForceIdleState();
                        }
                    
                        TeleportObjectToTarget(networkedUnitBehaviour, nextCellPosition);
                    }

                    break;
                }
                case (int)RaiseEventCode.OnRequestTeleport:
                {
                    object[] data = (object[]) photonEvent.CustomData;
                    int viewID = (int) data[0];

                    if (battleData.AllUnitsRuntimeSet.GetUnitByViewID(viewID, out NetworkedBattleBehaviour networkedUnitBehaviour))
                    {
                        Vector3Int targetCellPosition = (Vector3Int) data[1];
                        PerformGridTeleport(networkedUnitBehaviour, targetCellPosition);
                    }

                    break;
                }
            }
        }
        
        #endregion
    }
}
