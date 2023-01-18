using System.Collections;
using ExitGames.Client.Photon;
using Features.Battle.Scripts.StageProgression;
using Features.Battle.StateMachine;
using Features.Connection.Scripts.Utils;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Features.Battle.Scripts
{
    [CreateAssetMenu]
    public class StageSetupState : BaseBattleState_SO
    {
        public BattleData_SO battleData;
        public StageRandomizer_SO stageRandomizer;
        
        private BattleManager _battleManager;
        private bool _initialized;

        public StageSetupState Initialize(BattleManager battleManager)
        {
            if (_initialized) return this;
            
            _battleManager = battleManager;
            
            return this;
        }

        public override IEnumerator Enter()
        {
            yield return base.Enter();
            
            if (!battleData.IsStageRestart)
            {
                battleData.Stage.Add(1);
            }

            if (PhotonNetwork.IsMasterClient)
            {
                stageRandomizer.GenerateUnits();
            }

            if (PhotonNetwork.IsMasterClient)
            {
                RequestBattleStateByRaiseEvent();
            }
            
            Debug.Log("Enter Stage Setup State - Before");
            yield return new WaitForSeconds(2f);
            Debug.Log("Enter Stage Setup State - After");
        }

        public override IEnumerator Exit()
        {
            yield return base.Exit();

            Debug.Log("Exit Stage Setup State - Before");
            yield return new WaitForSeconds(2f);
            Debug.Log("Exit Stage Setup State - After");
        }

        public override void OnEvent(EventData photonEvent)
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
