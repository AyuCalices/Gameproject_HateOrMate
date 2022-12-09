using System.Collections.Generic;
using DataStructures.StateLogic;
using ExitGames.Client.Photon;
using Features.Battle.Scripts;
using Features.Mod.Action;
using Features.Tiles;
using Features.Unit.Battle.Scripts.Actions;
using Features.Unit.Modding;
using Features.Unit.View;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Features.Unit.Battle.Scripts
{
    [RequireComponent(typeof(NetworkedUnitBehaviour), typeof(PhotonView), typeof(UnitView))]
    public class BattleBehaviour : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        [SerializeField] private TileRuntimeDictionary_SO tileRuntimeDictionary;
        [SerializeField] private BattleData_SO battleData;
        [SerializeField] private BattleActionGenerator_SO battleActionsGenerator;
        [SerializeField] private bool isTargetable;
        [SerializeField] private float range;
        
        public NetworkedUnitBehaviour NetworkedUnitBehaviour { get; private set; }
        private StateMachine _stateMachine;
        private BattleActions _battleActions;
        private UnitView _unitView;

        public BattleActions BattleActions => _battleActions;
        public IState CurrentState => _stateMachine.CurrentState;

        private KeyValuePair<NetworkedUnitBehaviour, float> _closestUnit;


        public KeyValuePair<NetworkedUnitBehaviour, float> GetTarget => _closestUnit;
        public bool HasTarget { get; private set; }
        public bool TargetInRange => _closestUnit.Value < range;

        public bool IsTargetable
        {
            get => isTargetable;
            set
            {
                isTargetable = value;
                _unitView.SetHealthActive(value);
            }
        }

        private void Awake()
        {
            _stateMachine = new StateMachine();
            _stateMachine.Initialize(new IdleState(this));
            NetworkedUnitBehaviour = GetComponent<NetworkedUnitBehaviour>();
            
            _unitView = GetComponent<UnitView>();

            IsTargetable = isTargetable;
        }

        public void OnNetworkingEnabled()
        {
            _battleActions = battleActionsGenerator.Generate(NetworkedUnitBehaviour, this, _unitView);
        }

        public void OnStageEnd()
        {
            _battleActions.OnStageEnd();
        }

        private void Update()
        {
            if (battleData.CurrentState is not BattleState) return;
            
            HasTarget = NetworkedUnitBehaviour.EnemyRuntimeSet.TryGetClosestTargetableByWorldPosition(transform.position,
                    out _closestUnit);

            _stateMachine.Update();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="photonEvent"></param>
        public void OnEvent(EventData photonEvent)
        {
            if (battleData.CurrentState is not BattleState) return;
            
            //1st step: send damage + animation behaviour from attacker to calculating instance - Client & Master: Send to others
            if (photonEvent.Code == (int)RaiseEventCode.OnPerformUnitAttack)
            {
                object[] data = (object[]) photonEvent.CustomData;
                if (photonView.ViewID == (int) data[0])
                {
                    _battleActions.OnSendAttackActionCallback((float) data[1]);
                }
            }
            //2nd step: raise event to update health on all clients on attacked instance
            else if (photonEvent.Code == (int)RaiseEventCode.OnPerformUpdateUnitHealth)
            {
                object[] data = (object[]) photonEvent.CustomData;
                if (photonView.ViewID == (int) data[0])
                {
                    _battleActions.OnSendHealthActionCallback((float) data[1], (float) data[2]);
                }
            }
        }

        public void RequestIdleState()
        {
            _stateMachine.ChangeState(new IdleState(this));
        }

        public void RequestAttackState()
        {
            _stateMachine.ChangeState(new AttackState(this, _battleActions));
        }

        public void RequestMovementState()
        {
            _stateMachine.ChangeState(new MovementState(this, tileRuntimeDictionary));
        }
        
        public void RequestDeathState()
        {
            _stateMachine.ChangeState(new DeathState(this));
        }
    }
}
