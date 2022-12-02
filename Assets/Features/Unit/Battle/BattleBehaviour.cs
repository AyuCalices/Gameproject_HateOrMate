using System;
using DataStructures.StateLogic;
using ExitGames.Client.Photon;
using Features.Battle;
using Features.GlobalReferences;
using Features.Mod.Action;
using Features.Unit.Modding;
using Features.Unit.Modding.Stat;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Features.Unit.Battle
{
    [RequireComponent(typeof(NetworkedUnitBehaviour), typeof(PhotonView), typeof(UnitView))]
    public class BattleBehaviour : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        [SerializeField] private BattleActionGenerator_SO battleActionsGenerator;
        [SerializeField] private bool isTargetable;
        [SerializeField] private BattleData_SO battleData;
        [SerializeField] private float range;
        
        private NetworkedUnitBehaviour _networkedUnitBehaviour;
        private StateMachine _stateMachine;
        private BattleActions _battleActions;
        private UnitView _unitView;

        public bool IsTargetable => isTargetable;
        public BattleActions BattleActions => _battleActions;

        private float _removedHealth;

        public float RemovedHealth
        {
            get => _removedHealth;
            set
            {
                _removedHealth = value;
                _unitView.SetHealthSlider(_removedHealth, _networkedUnitBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Health));
            }
        }

        private void Awake()
        {
            _stateMachine = new StateMachine();
            _stateMachine.Initialize(new IdleState());
            _networkedUnitBehaviour = GetComponent<NetworkedUnitBehaviour>();
            
            _unitView = GetComponent<UnitView>();

            _removedHealth = 0;
        }

        private void Start()
        {
            _battleActions = battleActionsGenerator.Generate(_networkedUnitBehaviour, this, _unitView,
                battleData.EnemyUnitRuntimeSet);

            EnterAttackState();
        }

        private void Update()
        {
            /*
            switch (_stateMachine.CurrentState)
            {
                case MovementState when battleManager.EnemyUnitRuntimeSet.IsInRangeByWorldPosition(range, transform.position):
                    EnterAttackState();
                    break;
                case AttackState when !battleManager.EnemyUnitRuntimeSet.IsInRangeByWorldPosition(range, transform.position):
                    EnterMovementState();
                    break;
            }*/

            _stateMachine.Update();
        }
        
        
        
        //AI events gets called by masterClient: If Client Attack on AI -> Send Attack To Master
        //If Client attacks AI -> send attack to master -> update health by RaiseEvent
        //If AI attacks Client -> master sends attack to client -> update health by RaiseEvent
        
        //If Master attacks AI -> calculate attack locally -> update health by RaiseEvent
        //If AI attacks Master -> master calculated locally -> update health by RaiseEvent

        //more precise:
        //if isMaster: send own attacks and ai attacks to others | if client attacks -> master updates health for ai & own
        //if isClient: send attack always to master
        //update health by raise event always the same
        
        //this means: ai is networkedUnitBehaviour of master client
        //both player have networkedUnitBehaviour for ai if the host changes (only the host manages attacks)
        //enemy towers are idle towers - not clicker (maybe implement AI Actions acting - there also can be differentiated between Master and Client)
        //exchanging behaviours is a no go!

        /// <summary>
        /// 
        /// </summary>
        /// <param name="photonEvent"></param>
        public void OnEvent(EventData photonEvent)
        {
            //1st step: send damage + animation behaviour from attacker to calculating instance - Client & Master: Send to others
            if (photonEvent.Code == RaiseEventCode.OnPerformUnitAttack)
            {
                object[] data = (object[]) photonEvent.CustomData;
                if (photonView.ViewID == (int) data[0])
                {
                    _battleActions.OnSendAttackActionCallback((float) data[1]);
                }
            }
            //2nd step: raise event to update health on all clients on attacked instance
            else if (photonEvent.Code == RaiseEventCode.OnPerformUpdateUnitHealth)
            {
                object[] data = (object[]) photonEvent.CustomData;
                if (photonView.ViewID == (int) data[0])
                {
                    _battleActions.OnSendHealthActionCallback((float) data[1], (float) data[2]);
                }
            }
        }

        public void EnterIdleState()
        {
            _stateMachine.ChangeState(new IdleState());
        }

        public void EnterAttackState()
        {
            if (isTargetable)
            {
                _stateMachine.ChangeState(new AttackState(_battleActions));
            }
        }

        public void EnterMovementState()
        {
            _stateMachine.ChangeState(new MovementState(_battleActions));
        }
    }
}
