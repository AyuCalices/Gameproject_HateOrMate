using System;
using DataStructures.StateLogic;
using ExitGames.Client.Photon;
using Features.Loot.Scripts;
using Features.Unit.Battle.Scripts;
using Photon.Pun;
using Photon.Realtime;

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

            if (PhotonNetwork.IsMasterClient)
            {
                BattleManager.onNetworkedSpawnUnit?.Invoke("AiTower", _battleManager.aiTowerClass);
                BattleManager.onNetworkedSpawnUnit?.Invoke("AiTower", _battleManager.aiTowerClass);
                BattleManager.onNetworkedSpawnUnit?.Invoke("Gate", _battleManager.gateClass);
            }

            if (PhotonNetwork.IsMasterClient)
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
                _battleManager.RequestBattleState();
            }
        }

        private void RequestBattleStateByRaiseEvent()
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
            
            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnRequestBattleState, null, raiseEventOptions, sendOptions);
        }
    }
}
