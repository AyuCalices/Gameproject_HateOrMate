using DataStructures.StateLogic;
using ExitGames.Client.Photon;
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
        public TMP_Text StageText => stageText;
        public BattleData_SO BattleData => battleData;

        private void Awake()
        {
            _stageStateMachine = new StateMachine();
            battleData.RegisterBattleManager(this);
            battleData.Stage = 0;
        }

        private void Start()
        {
            _stageStateMachine.Initialize(new BattleSetupState(this, true));
            
            stageText.text = "Stage: " + battleData.Stage;
        }

        private void EnterPausedState(bool restartState)
        {
            _stageStateMachine.ChangeState(new BattleSetupState(this, restartState));
        }

        private void EnterRunningState()
        {
            _stageStateMachine.ChangeState(new RunningState(this));
        }

        public void StageCheck()
        {
            if (CurrentState is not RunningState) return;
            
            if (!battleData.PlayerTeamUnitRuntimeSet.HasUnitAlive())
            {
                EnterPausedState(true);
                return;
            }

            if (!battleData.EnemyUnitRuntimeSet.HasUnitAlive())
            {
                EnterPausedState(false);
            }
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
