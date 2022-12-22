using System;
using DataStructures.StateLogic;
using ExitGames.Client.Photon;
using Features.Loot.Scripts;
using Features.Unit.Battle.Scripts;
using Features.Unit.Classes;
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
        public static Action<string, UnitClassData_SO> onNetworkedSpawnUnit;
        public static Func<string, UnitClassData_SO, NetworkedBattleBehaviour> onLocalSpawnUnit;
        public static Action<string> onLocalDespawnAllUnits;
        
        public UnitClassData_SO gateClass;
        public UnitClassData_SO aiTowerClass;
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
            continueBattleButton.gameObject.SetActive(false);

            _enterLootingPhaseRoomDecision = new RoomDecisions<bool>("EnterLootingPhase", true);
            requestLootPhaseButton.onClick.AddListener(() => _enterLootingPhaseRoomDecision.SetLocalDecision(true));
        }

        private void Start()
        {
            onLocalSpawnUnit.Invoke("Player", towerClass);
            
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
            _stageStateMachine.ChangeState(new StageSetupState(this, restartState, battleData));
        }

        internal void RequestBattleState()
        {
            _stageStateMachine.ChangeState(new BattleState(this, battleData, battleData.AllUnitsRuntimeSet));
        }

        private void RequestLootingState(bool restartState)
        {
            _stageStateMachine.ChangeState(new LootingState(this, battleData, lootSelectionBehaviour, continueBattleButton, restartState));
        }

        public void EndStage(bool restartState)
        {
            if (!restartState)
            {
                LootingState.LootCount++;
            }
            
            onLocalDespawnAllUnits?.Invoke("AiTower");
            onLocalDespawnAllUnits?.Invoke("Gate");
            
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
