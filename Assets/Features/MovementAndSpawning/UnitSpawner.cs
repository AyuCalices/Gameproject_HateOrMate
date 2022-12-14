using System.Collections.Generic;
using ExitGames.Client.Photon;
using Features.Battle.Scripts;
using Features.Battle.Scripts.StageProgression;
using Features.Connection;
using Features.Connection.Scripts.Utils;
using Features.Unit.Scripts;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Behaviours.Stat;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Features.MovementAndSpawning
{
    public class UnitSpawner : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        [SerializeField] private List<SpawnerInstance> spawnerInstances;

        public override void OnEnable()
        {
            base.OnEnable();
            StageRandomizer_SO.onNetworkedSpawnUnit += PlayerSynchronizedSpawn;
            BattleManager.onLocalDespawnAllUnits += PlayerDespawnAll;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            StageRandomizer_SO.onNetworkedSpawnUnit -= PlayerSynchronizedSpawn;
            BattleManager.onLocalDespawnAllUnits -= PlayerDespawnAll;
        }
        
        private void PlayerSynchronizedSpawn(string spawnerReference, UnitClassData_SO unitClassData, BaseStats baseStats)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                SpawnHelper.SpawnUnit(spawnerInstances, PhotonNetwork.LocalPlayer.ActorNumber, spawnerReference, 
                    unitClassData, baseStats, (unit, position) =>
                {
                    int spawnerInstanceIndex = SpawnHelper.GetSpawnerInstanceIndex(spawnerInstances, spawnerReference);
                    PerformPlayerSynchronizedUnitInstantiation_RaiseEvent(unit.PhotonView.ViewID, position, PhotonNetwork.LocalPlayer.ActorNumber,
                        spawnerInstanceIndex, unitClassData, baseStats);
                    unit.NetworkedStatsBehaviour.OnPhotonViewIdAllocated();
                });
            }
            else
            {
                RequestPlayerSynchronizedUnitInstantiation_RaiseEvent(PhotonNetwork.LocalPlayer.ActorNumber, spawnerReference, unitClassData, baseStats);
            }
        }
        
        private void PlayerDespawnAll()
        {
            SpawnHelper.PlayerSynchronizedDespawnAll(spawnerInstances);
        }

        #region RaiseEvent Callbacks

        public void OnEvent(EventData photonEvent)
        {
            switch (photonEvent.Code)
            {
                case (int)RaiseEventCode.OnPerformUnitInstantiation:
                {
                    object[] data = (object[]) photonEvent.CustomData;

                    Vector3Int gridPosition = (Vector3Int) data[1];
                    int actorNumber = (int) data[2];
                    SpawnerInstance spawnerInstance = spawnerInstances[(int) data[3]];
                    UnitClassData_SO unitClassData = (UnitClassData_SO) data[4];
                    BaseStats baseStats = (BaseStats) data[5];
                
                    NetworkedBattleBehaviour player = spawnerInstance.InstantiateAndInitialize(actorNumber, unitClassData, 
                        baseStats, gridPosition, (int) data[3]);
                
                    player.PhotonView.ViewID = (int) data[0];
                    player.NetworkedStatsBehaviour.OnPhotonViewIdAllocated();
                    break;
                }
                case (int)RaiseEventCode.OnRequestUnitInstantiation:
                {
                    object[] data = (object[]) photonEvent.CustomData;
                    int actorNumber = (int) data[0];
                    string spawnerReference = (string) data[1];
                    UnitClassData_SO unitClassData = (UnitClassData_SO) data[2];
                    BaseStats baseStats = (BaseStats) data[3];
                
                    SpawnHelper.SpawnUnit(spawnerInstances, actorNumber, spawnerReference, unitClassData, baseStats, (unit, position) =>
                    {
                        int spawnerInstanceIndex = SpawnHelper.GetSpawnerInstanceIndex(spawnerInstances, spawnerReference);
                        PerformPlayerSynchronizedUnitInstantiation_RaiseEvent(unit.PhotonView.ViewID, position, actorNumber,
                            spawnerInstanceIndex, unitClassData, baseStats);
                        unit.NetworkedStatsBehaviour.OnPhotonViewIdAllocated();
                    });
                    break;
                }
            }
        }
        
        #endregion
        
        #region RaiseEvents: MasterClient sends result to all

        private static void PerformPlayerSynchronizedUnitInstantiation_RaiseEvent(int viewID, Vector3Int targetGridPosition, int photonActorNumber, 
            int spawnerInstanceIndex, UnitClassData_SO unitClassData, BaseStats baseStats)
        {
            object[] data = new object[]
            {
                viewID,
                targetGridPosition,
                photonActorNumber,
                spawnerInstanceIndex,
                unitClassData,
                baseStats
            };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.Others,
                CachingOption = EventCaching.AddToRoomCache
            };

            SendOptions sendOptions = new SendOptions
            {
                Reliability = true
            };

            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnPerformUnitInstantiation, data, raiseEventOptions, sendOptions);
        }
        
        #endregion

        #region RaiseEvents: Requests for MasterClient
        
        private static void RequestPlayerSynchronizedUnitInstantiation_RaiseEvent(int photonActorNumber, string spawnerReference, 
            UnitClassData_SO unitClassData, BaseStats baseStats)
        {
            object[] data = new object[]
            {
                photonActorNumber,
                spawnerReference,
                unitClassData,
                baseStats
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

            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnRequestUnitInstantiation, data, raiseEventOptions, sendOptions);
        }
        
        #endregion
    }
}
