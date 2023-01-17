using System;
using DataStructures.StateLogic;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.StructWrapping;
using Features.Battle.Scripts.StageProgression;
using Features.Connection;
using Features.Connection.Scripts;
using Features.Connection.Scripts.Utils;
using Features.Loot.Scripts;
using Features.Loot.Scripts.Generator;
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
        [SerializeField] private BattleData_SO battleData;
        [SerializeField] private ErrorPopup errorPopup;
        [SerializeField] private Transform errorPopupInstantiationParent;
        [SerializeField] private int lootCountOnStageComplete;
        
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
            
            _stageStateMachine.Initialize(new LootingState(this, battleData, errorPopup, errorPopupInstantiationParent, continueBattleButton, true));
            
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
            _stageStateMachine.ChangeState(new BattleState(this, requestLootPhaseButton, battleData.AllUnitsRuntimeSet));
        }

        private void RequestLootingState(bool restartState)
        {
            _stageStateMachine.ChangeState(new LootingState(this, battleData, errorPopup, errorPopupInstantiationParent, continueBattleButton, restartState));
        }

        public void SetStage()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            
            bool enterLootingState = _enterLootingPhaseRoomDecision.IsValidDecision(null, x => x);
            
            if (!battleData.PlayerUnitsRuntimeSet.HasUnitAlive())
            {
                RestartStage_RaiseEvent(enterLootingState);
                return;
            }

            if (!battleData.EnemyUnitsRuntimeSet.HasUnitAlive())
            {
                LootableGenerator_SO[] lootables = RandomizeLootables();
                NextStage_RaiseEvent(enterLootingState, lootables, battleData.Stage.Get());
            }
        }
        
        private void NextStage_RaiseEvent(bool enterLootingState, LootableGenerator_SO[] lootable, int currentStageAsLevel)
        {
            object[] data = new object[]
            {
                enterLootingState,
                lootable,
                currentStageAsLevel
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
            
            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnNextStage, data, raiseEventOptions, sendOptions);
        }
        
        private void RestartStage_RaiseEvent(bool enterLootingState)
        {
            object[] data = new object[]
            {
                enterLootingState
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
            
            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnRestartStage, data, raiseEventOptions, sendOptions);
        }

        public void EndStage(bool restartStage, bool enterLootingState)
        {
            onLocalDespawnAllUnits?.Invoke();
            
            foreach (NetworkedBattleBehaviour networkedUnitBehaviour in battleData.AllUnitsRuntimeSet.GetItems())
            {
                networkedUnitBehaviour.OnStageEnd();
                networkedUnitBehaviour.NetworkedStatsBehaviour.RemovedHealth = 0;
            }
            
            if (enterLootingState)
            {
                RequestLootingState(restartStage);
            }
            else
            {
                RequestStageSetupState(restartStage);
            }
        }

        private LootableGenerator_SO[] RandomizeLootables()
        {
            LootableGenerator_SO[] lootables = new LootableGenerator_SO[lootCountOnStageComplete];
            for (int index = 0; index < lootCountOnStageComplete; index++)
            {
                lootables[index] = battleData.LootTable.RandomizeLootableGenerator();
            }

            return lootables;
        }

        public void OnEvent(EventData photonEvent)
        {
            _stageStateMachine.OnEvent(photonEvent);
        }
    }
}
