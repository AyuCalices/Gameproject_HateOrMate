using System.Collections.Generic;
using ExitGames.Client.Photon;
using Features.BattleScene.Scripts.StageProgression;
using Features.General.Photon.Scripts;
using Features.Unit.Scripts;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Features.MovementAndSpawning.Scripts
{
    public class UnitSpawner : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        [SerializeField] private List<SpawnerInstance> spawnerInstances;

        public override void OnEnable()
        {
            base.OnEnable();
            StageRandomizer_SO.onNetworkedSpawnUnit += PlayerSynchronizedSpawn;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            StageRandomizer_SO.onNetworkedSpawnUnit -= PlayerSynchronizedSpawn;
        }
        
        private void PlayerSynchronizedSpawn(string spawnerReference, UnitClassData_SO unitClassData, int level)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                SpawnHelper.PhotonSpawnUnit(spawnerInstances, spawnerReference, unitClassData, level, false);
            }
            else
            {
                RequestPlayerSynchronizedUnitInstantiation_RaiseEvent(spawnerReference, unitClassData, level);
            }
        }

        #region RaiseEvent Callbacks

        public void OnEvent(EventData photonEvent)
        {
            switch (photonEvent.Code)
            {
                case (int)RaiseEventCode.OnRequestUnitInstantiation:
                {
                    object[] data = (object[]) photonEvent.CustomData;
                    string spawnerReference = (string) data[0];
                    UnitClassData_SO unitClassData = (UnitClassData_SO) data[1];
                    int level = (int) data[2];
                
                    SpawnHelper.PhotonSpawnUnit(spawnerInstances, spawnerReference, unitClassData, level, false);
                    break;
                }
            }
        }
        
        #endregion

        #region RaiseEvents: Requests for MasterClient
        
        private static void RequestPlayerSynchronizedUnitInstantiation_RaiseEvent(string spawnerReference, 
        UnitClassData_SO unitClassData, int level)
        {
            object[] data = new object[]
            {
                spawnerReference,
                unitClassData,
                level
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
