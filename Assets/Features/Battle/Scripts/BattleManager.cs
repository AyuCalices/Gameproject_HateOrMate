using System;
using DataStructures.StateLogic;
using ExitGames.Client.Photon;
using Features.Battle.Scripts.StageProgression;
using Features.Connection;
using Features.Connection.Scripts.Utils;
using Features.Loot.Scripts;
using Features.Loot.Scripts.LootView;
using Features.Unit.Scripts;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Behaviours.Stat;
using Photon.Pun;
using Photon.Realtime;
using Plugins.UniRx.Extensions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Battle.Scripts
{
    public class BattleManager : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        public static Func<string, UnitClassData_SO, BaseStats, NetworkedBattleBehaviour> onLocalSpawnUnit;
        public static Action onLocalDespawnAllUnits;

        [SerializeField] private StageRandomizer_SO stageRandomizer;
        public UnitClassData_SO towerClass;
        [SerializeField] private LootSelectionBehaviour lootSelectionBehaviour;
        [SerializeField] private BattleData_SO battleData;
        
        [SerializeField] private Button requestLootPhaseButton;
        [SerializeField] private Button continueBattleButton;
        
        //reactive stage text
        [SerializeField] private TextMeshProUGUI stageText;
    
        private StateMachine _stageStateMachine;

        public IState CurrentState => _stageStateMachine.CurrentState;

        private RoomDecisions<bool> _enterLootingPhaseRoomDecision;

        private void Awake()
        {
            _stageStateMachine = new StateMachine();
            battleData.RegisterBattleManager(this);
            continueBattleButton.interactable = false;

            _enterLootingPhaseRoomDecision = new RoomDecisions<bool>("EnterLootingPhase", true);
            requestLootPhaseButton.onClick.AddListener(() =>
            {
                _enterLootingPhaseRoomDecision.SetLocalDecision(true);
                requestLootPhaseButton.interactable = false;
            });
        }

        private void Start()
        {
            onLocalSpawnUnit.Invoke("Player", towerClass, new BaseStats(10, 50, 3));
            
            _stageStateMachine.Initialize(new LootingState(this, battleData, lootSelectionBehaviour, continueBattleButton, true));
            
            battleData.Stage.RuntimeProperty
                .Select(x => "Stage: " + x)
                .SubscribeToText(stageText);
        }

        private void Update()
        {
            _stageStateMachine.Update();
        }

        internal void RequestStageSetupState(bool restartState)
        {
            _stageStateMachine.ChangeState(new StageSetupState(this, restartState, battleData, stageRandomizer));
        }

        internal void RequestBattleState()
        {
            _stageStateMachine.ChangeState(new BattleState(this, battleData, requestLootPhaseButton, battleData.AllUnitsRuntimeSet));
        }

        private void RequestLootingState(bool restartState)
        {
            _stageStateMachine.ChangeState(new LootingState(this, battleData, lootSelectionBehaviour, continueBattleButton, restartState));
        }

        public void SetStage()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            
            if (!battleData.PlayerUnitsRuntimeSet.HasUnitAlive())
            {
                EndStage_RaiseEvent(true);
                return;
            }

            if (!battleData.EnemyUnitsRuntimeSet.HasUnitAlive())
            {
                EndStage_RaiseEvent(false);
            }
        }
        
        private void EndStage_RaiseEvent(bool restartStage)
        {
            object[] data = new object[]
            {
                restartStage
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
            
            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnEndStage, data, raiseEventOptions, sendOptions);
        }

        public void EndStage(bool restartState)
        {
            if (!restartState)
            {
                LootingState.LootCount++;
            }
            
            onLocalDespawnAllUnits?.Invoke();
            
            foreach (NetworkedBattleBehaviour networkedUnitBehaviour in battleData.AllUnitsRuntimeSet.GetItems())
            {
                networkedUnitBehaviour.OnStageEnd();
                networkedUnitBehaviour.NetworkedStatsBehaviour.RemovedHealth = 0;
            }

            bool enteredLootingState = _enterLootingPhaseRoomDecision.UpdateDecision(() => RequestLootingState(restartState));
            if (!enteredLootingState)
            {
                RequestStageSetupState(restartState);
            }
        }

        public void OnEvent(EventData photonEvent)
        {
            _stageStateMachine.OnEvent(photonEvent);
        }
    }
}
