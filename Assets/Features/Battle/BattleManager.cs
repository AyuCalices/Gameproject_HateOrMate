using DataStructures.StateLogic;
using Photon.Pun;
using UnityEngine;

namespace Features.Battle
{
    public class BattleManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private BattleData_SO battleData;
    
        private StateMachine _stageStateMachine;
    
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

        //TODO: prepare ai stats, current stage level
        //TODO: set removed health to 0
        //TODO: place units to their grid position
        //TODO: implement gained loot
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
