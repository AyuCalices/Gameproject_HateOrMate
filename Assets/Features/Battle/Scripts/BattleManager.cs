using System;
using ExitGames.Client.Photon;
using Features.Battle.StateMachine;
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

        public LootingState lootingState;
        public StageSetupState stageSetupState;
        public BattleState battleState;

        public UnitClassData_SO towerClass;
        [SerializeField] private BattleData_SO battleData;
        [SerializeField] private Transform errorPopupInstantiationParent;

        [SerializeField] private Button requestLootPhaseButton;
        [SerializeField] private Button continueBattleButton;
        
        //reactive stage text
        [SerializeField] private TextMeshProUGUI stageText;
    
        private BattleStateMachine _stageStateMachine;

        public IBattleState CurrentState => _stageStateMachine.CurrentState;

        
        private void Awake()
        {
            _stageStateMachine = new BattleStateMachine();
            battleData.Initialize(this);
            continueBattleButton.interactable = false;
        }

        private void Start()
        {
            onLocalSpawnUnit.Invoke("Player", towerClass, new BaseStats(10, 50, 3));
            
            _stageStateMachine.Initialize(lootingState.Initialize(this, errorPopupInstantiationParent, continueBattleButton));
            
            battleData.Stage.RuntimeProperty
                .Select(x => "Stage: " + x)
                .SubscribeToText(stageText);
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            _stageStateMachine.OnRoomPropertiesUpdated(propertiesThatChanged);
        }

        internal void RequestStageSetupState()
        {
            _stageStateMachine.ChangeState(stageSetupState.Initialize(this));
        }

        internal void RequestBattleState()
        {
            _stageStateMachine.ChangeState(battleState.Initialize(this, requestLootPhaseButton));
        }

        internal void RequestLootingState()
        {
            _stageStateMachine.ChangeState(lootingState.Initialize(this, errorPopupInstantiationParent, continueBattleButton));
        }

        public void OnEvent(EventData photonEvent)
        {
            _stageStateMachine.OnEvent(photonEvent);
        }
    }
}
