using System;
using DataStructures.StateLogic;
using ExitGames.Client.Photon;
using Features.Loot.Scripts;
using Features.Unit.Battle.Scripts;
using Features.Unit.Classes;
using Photon.Pun;
using Photon.Realtime;

namespace Features.Battle.Scripts
{
    public class StageSetupState : IState
    {
        public static Action<string, UnitClassData_SO> onNetworkedSpawnUnit;
        public static Action<string> onLocalDespawnAllUnits;
        
        private readonly BattleData_SO _battleData;
        private readonly BattleManager _battleManager;
        private readonly bool _restartStage;
        
        public StageSetupState(BattleManager battleManager, bool restartStage, BattleData_SO battleData)
        {
            _battleManager = battleManager;
            _restartStage = restartStage;
            _battleData = battleData;
        }
    
        public void Enter()
        {
            if (!_restartStage)
            {
                _battleData.Stage.Add(1);
            }

            foreach (NetworkedBattleBehaviour networkedUnitBehaviour in _battleData.AllUnitsRuntimeSet.GetItems())
            {
                networkedUnitBehaviour.OnStageEnd();
                networkedUnitBehaviour.NetworkedStatsBehaviour.RemovedHealth = 0;
            }

            if (PhotonNetwork.IsMasterClient && !_restartStage)
            {
                SendLootableByRaiseEvent(_battleData.LootTable.RandomizeLootableGenerator());
            }

            onLocalDespawnAllUnits?.Invoke("RightTower");
            onLocalDespawnAllUnits?.Invoke("LeftTower");
            onLocalDespawnAllUnits?.Invoke("Gate");
            if (PhotonNetwork.IsMasterClient)
            {
                onNetworkedSpawnUnit?.Invoke("RightTower", _battleManager.aiTowerClass);
                onNetworkedSpawnUnit?.Invoke("LeftTower", _battleManager.aiTowerClass);
                onNetworkedSpawnUnit?.Invoke("Gate", _battleManager.gateClass);
            }

            if (!PhotonNetwork.IsMasterClient || PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                RequestBattleStateByRaiseEvent();
            }
        }

        public void Execute()
        {
        }

        public void Exit()
        {
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == (int)RaiseEventCode.OnRequestBattleState)
            {
                object[] data = (object[]) photonEvent.CustomData;
                bool isLootingState = (bool) data[0] || _battleManager.IsLootPhaseRequested;
                SetBattleStateByRaiseEvent(isLootingState);
            }
            
            if (photonEvent.Code == (int)RaiseEventCode.OnSetBattleState)
            {
                object[] data = (object[]) photonEvent.CustomData;
                bool isLootingState = (bool) data[0];
                if (isLootingState)
                {
                    _battleManager.RequestLootingState();
                }
                else
                {
                    _battleManager.RequestBattleState();
                }
            }
        }
        
        private void SetBattleStateByRaiseEvent(bool isLootingState)
        {
            object[] data = new object[]
            {
                isLootingState
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
            
            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnSetBattleState, data, raiseEventOptions, sendOptions);
        }
        
        private void SendLootableByRaiseEvent(LootableGenerator_SO lootable)
        {
            object[] data = new object[]
            {
                lootable
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
            
            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnObtainLoot, data, raiseEventOptions, sendOptions);
        }
        
        private void RequestBattleStateByRaiseEvent()
        {
            object[] data = new object[]
            {
                _battleManager.IsLootPhaseRequested
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
            
            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnRequestBattleState, data, raiseEventOptions, sendOptions);
        }
    }
}
