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
        private readonly StageRandomizer_SO _stageRandomizer;
        private readonly BattleManager _battleManager;
        private readonly bool _restartStage;
        
        public StageSetupState(BattleManager battleManager, bool restartStage, BattleData_SO battleData, StageRandomizer_SO stageRandomizer)
        {
            _battleManager = battleManager;
            _restartStage = restartStage;
            _battleData = battleData;
            _stageRandomizer = stageRandomizer;
        }
    
        public void Enter()
        {
            if (!_restartStage)
            {
                _battleData.Stage.Add(1);
            }

            if (PhotonNetwork.IsMasterClient)
            {
                _stageRandomizer.GenerateUnits();
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
