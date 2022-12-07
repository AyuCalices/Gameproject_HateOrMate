using DataStructures.StateLogic;
using ExitGames.Client.Photon;
using Features.Loot;
using Features.Mod;
using Features.Unit.Battle;
using Features.Unit.Modding;
using Photon.Pun;
using Photon.Realtime;

namespace Features.Battle
{
    public class StageSetupState : IState
    {
        private BattleManager _battleManager;
        private readonly LootGenerator_SO _lootGenerator;
        private bool _restartStage;
        
        public StageSetupState(BattleManager battleManager, LootGenerator_SO lootGenerator, bool restartStage)
        {
            _battleManager = battleManager;
            _lootGenerator = lootGenerator;
            _restartStage = restartStage;
        }
    
        public void Enter()
        {
            if (!_restartStage)
            {
                _battleManager.BattleData.Stage += 1;
            }
            
            _battleManager.StageText.text = "Stage: " + _battleManager.BattleData.Stage;
            
            //Debug.Log(battleData.PlayerTeamUnitRuntimeSet.GetItems().Count);
            foreach (NetworkedUnitBehaviour networkedUnitBehaviour in _battleManager.BattleData.PlayerTeamUnitRuntimeSet.GetItems())
            {
                if (networkedUnitBehaviour.TryGetComponent(out BattleBehaviour battleBehaviour))
                {
                    //TODO: switching to idle state while unit is moving by drag and drop will continue to make unit move cause its command based and not in the state machine. Might result in problems when ai-move
                    battleBehaviour.RequestIdleState();
                    battleBehaviour.OnStageEnd();
                }
                
                networkedUnitBehaviour.RemovedHealth = 0;
            }

            //Debug.Log(battleData.EnemyUnitRuntimeSet.GetItems().Count);
            foreach (NetworkedUnitBehaviour networkedUnitBehaviour in _battleManager.BattleData.EnemyUnitRuntimeSet.GetItems())
            {
                if (networkedUnitBehaviour.TryGetComponent(out BattleBehaviour battleBehaviour))
                {
                    battleBehaviour.RequestIdleState();
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
                SendLootableByRaiseEvent(_lootGenerator.GenerateNew());
            }

            if (!PhotonNetwork.IsMasterClient || PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                RequestBattleStateByRaiseEvent();
            }
        }
        
        private void SendLootableByRaiseEvent(BaseMod lootable)
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
