using DataStructures.ReactiveVariable;
using DataStructures.StateLogic;
using ExitGames.Client.Photon;
using Features.GlobalReferences.Scripts;
using Features.Loot.Scripts;
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
        [SerializeField] private LootTable_SO lootTable;
        [SerializeField] private LootSelectionBehaviour lootSelectionBehaviour;
        [SerializeField] private NetworkedUnitRuntimeSet_SO allUnitsRuntimeSet;
        [SerializeField] private BattleData_SO battleData;
        
        [SerializeField] private Toggle requestLootPhaseToggle;
        [SerializeField] private Button continueBattleButton;
        
        //reactive stage text
        [SerializeField] private TextMeshProUGUI stageText;
        [SerializeField] public IntReactiveVariable stage;
    
        private StateMachine _stageStateMachine;

        public bool IsLootPhaseRequested => requestLootPhaseToggle.isOn;
        public void DisableLootPhaseRequested() => requestLootPhaseToggle.isOn = false;
        
        public IState CurrentState => _stageStateMachine.CurrentState;
        public BattleData_SO BattleData => battleData;

        private void Awake()
        {
            _stageStateMachine = new StateMachine();
            battleData.RegisterBattleManager(this);
            DisableLootPhaseRequested();
            continueBattleButton.gameObject.SetActive(false);
        }

        private void Start()
        {
            _stageStateMachine.Initialize(new StageSetupState(this, lootTable, true, battleData));
            
            stage.RuntimeProperty
                .Select(x => "Stage: " + x)
                .SubscribeToText(stageText);
        }

        private void Update()
        {
            _stageStateMachine.Update();
        }

        internal void RequestStageSetupState(bool restartState)
        {
            _stageStateMachine.ChangeState(new StageSetupState(this, lootTable, restartState, battleData));
        }

        internal void RequestBattleState()
        {
            _stageStateMachine.ChangeState(new BattleState(this, battleData, allUnitsRuntimeSet));
        }
        
        internal void RequestLootingState()
        {
            _stageStateMachine.ChangeState(new LootingState(this, lootSelectionBehaviour, continueBattleButton));
        }

        public void OnEvent(EventData photonEvent)
        {
            _stageStateMachine.OnEvent(photonEvent);
        }
    }
}
