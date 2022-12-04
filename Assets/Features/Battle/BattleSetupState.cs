using DataStructures.StateLogic;
using ExitGames.Client.Photon;
using Features.Unit.Battle;
using Features.Unit.Modding;
using Photon.Pun;
using Photon.Realtime;

namespace Features.Battle
{
    public class BattleSetupState : IState
    {
        private BattleManager _battleManager;
        private bool _restartStage;
        
        public BattleSetupState(BattleManager battleManager, bool restartStage)
        {
            _battleManager = battleManager;
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

            EnterBattleByRaiseEvent();
        }
        
        private void EnterBattleByRaiseEvent()
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.All,
                CachingOption = EventCaching.AddToRoomCache
            };

            SendOptions sendOptions = new SendOptions
            {
                Reliability = true
            };

            object[] data = new object[] {};
            
            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnStartBattle, data, raiseEventOptions, sendOptions);
        }

        public void Execute()
        {
        }

        public void Exit()
        {
        }
    }
}
