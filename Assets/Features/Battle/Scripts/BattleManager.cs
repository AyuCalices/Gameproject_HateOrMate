using System;
using ExitGames.Client.Photon;
using Features.Battle.StateMachine;
using Features.Unit.Scripts;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Behaviours.Stat;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Features.Battle.Scripts
{
    public class BattleManager : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        public static Func<string, UnitClassData_SO, BaseStats, NetworkedBattleBehaviour> onLocalSpawnUnit;
        
        [SerializeField] private UnitClassData_SO towerClass;
        [SerializeField] private BattleData_SO battleData;
        [SerializeField] private PlacementState placementState;
        [SerializeField] private LootingState lootingState;
        [SerializeField] private StageSetupState stageSetupState;
        [SerializeField] private BattleState battleState;
        
        private CoroutineStateMachine _stageStateMachine;

        private void Awake()
        {
            _stageStateMachine = new CoroutineStateMachine();
            battleData.Initialize(this);
        }

        private void Start()
        {
            _stageStateMachine.Initialize(placementState.Initialize(this));
            
            //TODO: magic Numbers
            onLocalSpawnUnit.Invoke("Player", towerClass, new BaseStats(10, 50, 3));
        }
        
        public bool StateIsValid(Type checkedType, StateProgressType checkedStateProgressType)
        {
            return _stageStateMachine.StateIsValid(checkedType, checkedStateProgressType);
        }

        internal void RequestStageSetupState()
        {
            _stageStateMachine.ChangeState(stageSetupState.Initialize(this));
        }

        internal void RequestBattleState()
        {
            _stageStateMachine.ChangeState(battleState.Initialize(this));
        }

        internal void RequestLootingState()
        {
            _stageStateMachine.ChangeState(lootingState.Initialize(this));
        }
        
        internal void RequestPlacementState()
        {
            _stageStateMachine.ChangeState(placementState.Initialize(this));
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            _stageStateMachine.OnRoomPropertiesUpdated(propertiesThatChanged);
        }
        
        public void OnEvent(EventData photonEvent)
        {
            _stageStateMachine.OnEvent(photonEvent);
        }
    }
}
