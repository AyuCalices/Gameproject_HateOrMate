using System;
using ExitGames.Client.Photon;
using Features.Battle.StateMachine;
using Features.Connection.Scripts;
using Features.Loot.Scripts.ModView;
using Features.Unit.Scripts;
using Features.Unit.Scripts.Behaviours.Battle;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Features.Battle.Scripts
{
    public class BattleManager : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        public static Func<string, UnitClassData_SO, int, NetworkedBattleBehaviour> onLocalSpawnUnit;
        
        [SerializeField] private UnitClassData_SO towerClass;
        [SerializeField] private BattleData_SO battleData;
        [SerializeField] private PlacementState placementState;
        [SerializeField] private LootingState lootingState;
        [SerializeField] private StageSetupState stageSetupState;
        [SerializeField] private BattleState battleState;
        [SerializeField] private EndGameState endGameState;
        [SerializeField] private ErrorPopup errorPopupPrefab;
        [SerializeField] private CanvasFocus_SO canvasFocus;
        [SerializeField] private MusicBehaviour musicBehaviour;
        
        private CoroutineStateMachine _stageStateMachine;

        private void Awake()
        {
            _stageStateMachine = new CoroutineStateMachine();
            battleData.Initialize(this);
            musicBehaviour.Enable();
        }

        private void Start()
        {
            _stageStateMachine.Initialize(placementState.Initialize(this));
            
            onLocalSpawnUnit.Invoke("Player", towerClass, 0);
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
        
        public void RequestEndGameState()
        {
            _stageStateMachine.ChangeState(endGameState.Initialize(musicBehaviour));
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            _stageStateMachine.OnRoomPropertiesUpdated(propertiesThatChanged);
        }
        
        public void OnEvent(EventData photonEvent)
        {
            _stageStateMachine.OnEvent(photonEvent);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            if (cause == DisconnectCause.DisconnectByClientLogic)
            {
                SceneManager.LoadScene("ConnectionScreen");
            }
            else
            {
                errorPopupPrefab.Instantiate(canvasFocus.Get().transform, "Error: " + cause, RequestEndGameState);
            }
        }
        
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (StateIsValid(typeof(EndGameState), StateProgressType.All)) return;
            
            errorPopupPrefab.Instantiate(canvasFocus.Get().transform, "Error: Your Mate left the Room!", RequestEndGameState);
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
