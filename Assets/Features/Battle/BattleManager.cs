using System;
using DataStructures.StateLogic;
using Features.Unit.Battle;
using Features.Unit.Modding;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace Features.Battle
{
    public class BattleManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private BattleData_SO battleData;
        [SerializeField] private TMP_Text stageText;
    
        private StateMachine _stageStateMachine;
    
        private void Awake()
        {
            _stageStateMachine = new StateMachine();
            _stageStateMachine.Initialize(new PausedState(this));

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
            if (!battleData.PlayerTeamUnitRuntimeSet.HasUnitAlive())
            {
                Debug.Log("restart stage");
                RestartStage();
                return;
            }

            if (!battleData.EnemyUnitRuntimeSet.HasUnitAlive())
            {
                Debug.Log("next stage");
                NextStage();
            }
        }

        private void NextStage()
        {
            battleData.Stage += 1;
            stageText.text = "Stage: " + battleData.Stage;
            
            //Debug.Log(battleData.PlayerTeamUnitRuntimeSet.GetItems().Count);
            foreach (NetworkedUnitBehaviour networkedUnitBehaviour in battleData.PlayerTeamUnitRuntimeSet.GetItems())
            {
                if (networkedUnitBehaviour.TryGetComponent(out BattleBehaviour battleBehaviour))
                {
                    battleBehaviour.RequestIdleState();
                }
                
                networkedUnitBehaviour.RemovedHealth = 0;
            }

            //Debug.Log(battleData.EnemyUnitRuntimeSet.GetItems().Count);
            foreach (NetworkedUnitBehaviour networkedUnitBehaviour in battleData.EnemyUnitRuntimeSet.GetItems())
            {
                if (networkedUnitBehaviour.TryGetComponent(out BattleBehaviour battleBehaviour))
                {
                    battleBehaviour.RequestIdleState();
                }
                
                networkedUnitBehaviour.RemovedHealth = 0;
                
                if (networkedUnitBehaviour is AIUnitBehaviour aiUntBehaviour)
                {
                    battleData.SetAiStats(aiUntBehaviour);
                }
            }
        }

        private void RestartStage()
        {
            stageText.text = "Stage: " + battleData.Stage;
            
            foreach (NetworkedUnitBehaviour networkedUnitBehaviour in battleData.PlayerTeamUnitRuntimeSet.GetItems())
            {
                if (networkedUnitBehaviour.TryGetComponent(out BattleBehaviour battleBehaviour))
                {
                    battleBehaviour.RequestIdleState();
                }
                
                networkedUnitBehaviour.RemovedHealth = 0;
            }

            foreach (NetworkedUnitBehaviour networkedUnitBehaviour in battleData.EnemyUnitRuntimeSet.GetItems())
            {
                if (networkedUnitBehaviour.TryGetComponent(out BattleBehaviour battleBehaviour))
                {
                    battleBehaviour.RequestIdleState();
                }
                
                networkedUnitBehaviour.RemovedHealth = 0;
                
                if (networkedUnitBehaviour is AIUnitBehaviour aiUntBehaviour)
                {
                    battleData.SetAiStats(aiUntBehaviour);
                }
            }
        }
    }
}
