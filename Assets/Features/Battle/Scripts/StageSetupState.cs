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
        private BattleManager _battleManager;
        private readonly LootTable_SO _lootTable;
        private bool _restartStage;
        
        public StageSetupState(BattleManager battleManager, LootTable_SO lootTable, bool restartStage)
        {
            _battleManager = battleManager;
            _lootTable = lootTable;
            _restartStage = restartStage;
        }
    
        public void Enter()
        {
            if (!_restartStage)
            {
                _battleManager.stage.Add(1);
            }
            
            //Debug.Log(battleData.PlayerTeamUnitRuntimeSet.GetItems().Count);
            foreach (NetworkedUnitBehaviour networkedUnitBehaviour in _battleManager.BattleData.PlayerTeamUnitRuntimeSet.GetItems())
            {
                if (networkedUnitBehaviour.TryGetComponent(out BattleBehaviour battleBehaviour))
                {
                    battleBehaviour.OnStageEnd();
                }
                
                networkedUnitBehaviour.RemovedHealth = 0;
            }

            //Debug.Log(battleData.EnemyUnitRuntimeSet.GetItems().Count);
            foreach (NetworkedUnitBehaviour networkedUnitBehaviour in _battleManager.BattleData.EnemyUnitRuntimeSet.GetItems())
            {
                if (networkedUnitBehaviour.TryGetComponent(out BattleBehaviour battleBehaviour))
                {
                    battleBehaviour.OnStageEnd();
                }

                if (networkedUnitBehaviour is AIUnitBehaviour aiUntBehaviour)
                {
                    _battleManager.BattleData.SetAiStats(aiUntBehaviour);
                }
                
                networkedUnitBehaviour.RemovedHealth = 0;
            }

            if (PhotonNetwork.IsMasterClient && !_restartStage)
            {
                SendLootableByRaiseEvent(_lootTable.RandomizeLootableGenerator());
            }

            if (!PhotonNetwork.IsMasterClient || PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                RequestBattleStateByRaiseEvent();
            }
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

        public void Execute()
        {
        }

        public void Exit()
        {
        }
    }
}
