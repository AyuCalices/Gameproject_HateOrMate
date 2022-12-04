using DataStructures.StateLogic;
using ExitGames.Client.Photon;
using Features.Unit.Battle;
using Features.Unit.Modding;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace Features.Battle
{
    public class BattleManager : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        [SerializeField] private BattleData_SO battleData;
        [SerializeField] private TMP_Text stageText;
    
        private StateMachine _stageStateMachine;

        public IState CurrentState => _stageStateMachine.CurrentState;
    
        private void Awake()
        {
            _stageStateMachine = new StateMachine();
            _stageStateMachine.Initialize(new RunningState(this));

            battleData.Stage = 0;
            stageText.text = "Stage: " + battleData.Stage;
            battleData.RegisterBattleManager(this);
        }
    
        public void EnterPausedState()
        {
            _stageStateMachine.ChangeState(new PausedState(this));
        }

        public void EnterRunningState()
        {
            _stageStateMachine.ChangeState(new RunningState(this));
        }

        public void StageCheck()
        {
            Debug.Log(CurrentState);
            if (CurrentState is not RunningState) return;
            
            if (!battleData.PlayerTeamUnitRuntimeSet.HasUnitAlive())
            {
                RestartStage();
                return;
            }

            if (!battleData.EnemyUnitRuntimeSet.HasUnitAlive())
            {
                NextStage();
            }
        }

        private void NextStage()
        {
            EnterPausedState();
            battleData.Stage += 1;
            stageText.text = "Stage: " + battleData.Stage;
            
            //Debug.Log(battleData.PlayerTeamUnitRuntimeSet.GetItems().Count);
            foreach (NetworkedUnitBehaviour networkedUnitBehaviour in battleData.PlayerTeamUnitRuntimeSet.GetItems())
            {
                if (networkedUnitBehaviour.TryGetComponent(out BattleBehaviour battleBehaviour))
                {
                    battleBehaviour.RequestIdleState();
                    battleBehaviour.OnStageEnd();
                }
                
                networkedUnitBehaviour.RemovedHealth = 0;
            }

            //Debug.Log(battleData.EnemyUnitRuntimeSet.GetItems().Count);
            foreach (NetworkedUnitBehaviour networkedUnitBehaviour in battleData.EnemyUnitRuntimeSet.GetItems())
            {
                if (networkedUnitBehaviour.TryGetComponent(out BattleBehaviour battleBehaviour))
                {
                    battleBehaviour.RequestIdleState();
                    battleBehaviour.OnStageEnd();
                }
                
                networkedUnitBehaviour.RemovedHealth = 0;
                
                if (networkedUnitBehaviour is AIUnitBehaviour aiUntBehaviour)
                {
                    battleData.SetAiStats(aiUntBehaviour);
                }
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

        private void RestartStage()
        {
            EnterPausedState();
            stageText.text = "Stage: " + battleData.Stage;
            
            foreach (NetworkedUnitBehaviour networkedUnitBehaviour in battleData.PlayerTeamUnitRuntimeSet.GetItems())
            {
                if (networkedUnitBehaviour.TryGetComponent(out BattleBehaviour battleBehaviour))
                {
                    battleBehaviour.RequestIdleState();
                    battleBehaviour.OnStageEnd();
                }
                
                networkedUnitBehaviour.RemovedHealth = 0;
            }

            foreach (NetworkedUnitBehaviour networkedUnitBehaviour in battleData.EnemyUnitRuntimeSet.GetItems())
            {
                if (networkedUnitBehaviour.TryGetComponent(out BattleBehaviour battleBehaviour))
                {
                    battleBehaviour.RequestIdleState();
                    battleBehaviour.OnStageEnd();
                }
                
                networkedUnitBehaviour.RemovedHealth = 0;
                
                if (networkedUnitBehaviour is AIUnitBehaviour aiUntBehaviour)
                {
                    battleData.SetAiStats(aiUntBehaviour);
                }
            }
            
            EnterBattleByRaiseEvent();
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == (int)RaiseEventCode.OnStartBattle)
            {
                EnterRunningState();
            }
        }
    }
}
