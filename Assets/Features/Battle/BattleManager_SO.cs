using System;
using DataStructures.StateLogic;
using Features.GlobalReferences;
using UnityEngine;

namespace Features.Battle
{
    [CreateAssetMenu]
    public class BattleManager_SO : ScriptableObject
    {
        [SerializeField] private NetworkedUnitRuntimeSet_SO enemyUnitRuntimeSet;

        [SerializeField] private LocalUnitRuntimeSet_SO teamUnitRuntimeSet;

        public NetworkedUnitRuntimeSet_SO EnemyUnitRuntimeSet => enemyUnitRuntimeSet;
        public LocalUnitRuntimeSet_SO TeamUnitRuntimeSet => teamUnitRuntimeSet;

        private StateMachine _stageStateMachine;
        
        //TODO: correct battleManager initialisation - initializing of stages inside stageStateMachine
        //TODO: add rewards for stage completing in RuntimeList & add event for UI
        //TODO: update stage UI by event

        private void Awake()
        {
            _stageStateMachine = new StateMachine();
            _stageStateMachine.Initialize(new PausedState(this));
        }

        public void EnterPausedState()
        {
            _stageStateMachine.ChangeState(new PausedState(this));
        }

        public void EnterRunningState()
        {
            _stageStateMachine.ChangeState(new RunningState(this));
        }

        public void NextStage()
        {
            //TODO: implement here
        }

        public void RestartStage()
        {
            //TODO: implement here
        }
    }
}
