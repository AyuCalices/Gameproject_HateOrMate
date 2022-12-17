using DataStructures.StateLogic;
using ExitGames.Client.Photon;
using Features.Loot;
using Features.Loot.Scripts;
using Features.Unit.Battle;
using Features.Unit.Battle.Scripts;
using Features.Unit.Modding;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Features.Battle.Scripts
{
    public class StageSetupState : IState
    {
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
            
            //Debug.Log(battleData.PlayerTeamUnitRuntimeSet.GetItems().Count);
            foreach (NetworkedUnitBehaviour networkedUnitBehaviour in _battleData.PlayerUnitsRuntimeSet.GetItems())
            {
                if (networkedUnitBehaviour.TryGetComponent(out NetworkedBattleBehaviour battleBehaviour))
                {
                    battleBehaviour.OnStageEnd();
                }
                
                networkedUnitBehaviour.RemovedHealth = 0;
            }

            //Debug.Log(battleData.EnemyUnitRuntimeSet.GetItems().Count);
            foreach (NetworkedUnitBehaviour networkedUnitBehaviour in _battleData.EnemyUnitsRuntimeSet.GetItems())
            {
                if (networkedUnitBehaviour.TryGetComponent(out NetworkedBattleBehaviour battleBehaviour))
                {
                    battleBehaviour.OnStageEnd();
                }

                if (networkedUnitBehaviour is AIUnitBehaviour aiUntBehaviour)
                {
                    _battleData.SetAiStats(aiUntBehaviour);
                }
                
                networkedUnitBehaviour.RemovedHealth = 0;
            }

            if (PhotonNetwork.IsMasterClient && !_restartStage)
            {
                SendLootableByRaiseEvent(_battleData.LootTable.RandomizeLootableGenerator());
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
