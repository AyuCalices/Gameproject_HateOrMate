using System;
using DataStructures.StateLogic;
using ExitGames.Client.Photon;
using Features.Loot;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Battle
{
    public class BattleManager : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        [SerializeField] private LootGenerator_SO lootGenerator;
        [SerializeField] private LootSelectionBehaviour lootSelectionBehaviour;
        [SerializeField] private BattleData_SO battleData;
        [SerializeField] private TMP_Text stageText;
        [SerializeField] private Toggle requestLootPhaseToggle;
        [SerializeField] private Button continueBattleButton;
    
        private StateMachine _stageStateMachine;

        public bool IsLootPhaseRequested => requestLootPhaseToggle.isOn;
        public void DisableLootPhaseRequested() => requestLootPhaseToggle.isOn = false;
        
        public IState CurrentState => _stageStateMachine.CurrentState;
        public TMP_Text StageText => stageText;
        public BattleData_SO BattleData => battleData;

        private void Awake()
        {
            _stageStateMachine = new StateMachine();
            battleData.RegisterBattleManager(this);
            battleData.Stage = 0;
            DisableLootPhaseRequested();
            continueBattleButton.gameObject.SetActive(false);
        }

        private void Start()
        {
            _stageStateMachine.Initialize(new StageSetupState(this, lootGenerator, true));
            
            stageText.text = "Stage: " + battleData.Stage;
        }

        private void Update()
        {
            _stageStateMachine.Update();
        }

        private void RequestStageSetupState(bool restartState)
        {
            _stageStateMachine.ChangeState(new StageSetupState(this, lootGenerator, restartState));
        }

        public void RequestBattleState()
        {
            _stageStateMachine.ChangeState(new BattleState(this));
        }
        
        private void RequestLootingState()
        {
            _stageStateMachine.ChangeState(new LootingState(this, lootSelectionBehaviour, continueBattleButton));
        }

        public void StageCheck()
        {
            if (CurrentState is not BattleState) return;
            
            if (!battleData.PlayerTeamUnitRuntimeSet.HasUnitAlive())
            {
                RequestStageSetupState(true);
                return;
            }

            if (!battleData.EnemyUnitRuntimeSet.HasUnitAlive())
            {
                RequestStageSetupState(false);
            }
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == (int)RaiseEventCode.OnRequestBattleState)
            {
                object[] data = (object[]) photonEvent.CustomData;
                bool isLootingState = (bool) data[0] || IsLootPhaseRequested;
                SetBattleStateByRaiseEvent(isLootingState);
            }

            if (photonEvent.Code == (int)RaiseEventCode.OnSetBattleState)
            {
                object[] data = (object[]) photonEvent.CustomData;
                bool isLootingState = (bool) data[0];
                if (isLootingState)
                {
                    RequestLootingState();
                }
                else
                {
                    RequestBattleState();
                }
            }

            if (photonEvent.Code == (int)RaiseEventCode.OnObtainLoot)
            {
                object[] data = (object[]) photonEvent.CustomData;
                
                battleData.lootables.Add((LootableGenerator_SO)data[0]);
            }
        }
        
        private void SetBattleStateByRaiseEvent(bool isLootingState)
        {
            object[] data = new object[]
            {
                isLootingState
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
            
            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnSetBattleState, data, raiseEventOptions, sendOptions);
        }
    }
}
