using System.Collections.Generic;
using DataStructures.StateLogic;
using ExitGames.Client.Photon;
using Features.Battle;
using Features.Mod.Action;
using Features.Unit.Battle.Actions;
using Features.Unit.Modding;
using Features.Unit.View;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Features.Unit.Battle
{
    [RequireComponent(typeof(NetworkedUnitBehaviour), typeof(PhotonView), typeof(UnitView))]
    public class BattleBehaviour : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        public DamageProjectileBehaviour damageProjectilePrefab;
        [SerializeField] private BattleActionGenerator_SO battleActionsGenerator;
        [SerializeField] private bool isTargetable;
        [SerializeField] private float range;
        
        public NetworkedUnitBehaviour NetworkedUnitBehaviour { get; private set; }
        private StateMachine _stateMachine;
        private BattleActions _battleActions;
        private UnitView _unitView;

        public BattleActions BattleActions => _battleActions;
        public IState CurrentState => _stateMachine.CurrentState;

        private bool _hasTargetableEnemy;
        private NetworkedUnitBehaviour _closestUnit;

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
            _stateMachine.Initialize(new IdleState());
            NetworkedUnitBehaviour = GetComponent<NetworkedUnitBehaviour>();
            
            _unitView = GetComponent<UnitView>();

            IsTargetable = isTargetable;
        }

        private void Start()
        {
            _battleActions = battleActionsGenerator.Generate(NetworkedUnitBehaviour, this, _unitView);

            RequestAttackState();
        }

        private void Update()
        {
            _hasTargetableEnemy = NetworkedUnitBehaviour.EnemyRuntimeSet.TryGetClosestTargetableByWorldPosition(transform.position,
                    out KeyValuePair<NetworkedUnitBehaviour, float> closestUnit);
            _closestUnit = closestUnit.Key;

            switch (CurrentState)
            {
                case AttackState when !_hasTargetableEnemy:
                    RequestIdleState();
                    break;
                case AttackState or IdleState when _hasTargetableEnemy && closestUnit.Value > range:
                    RequestMovementState();
                    break;
                case MovementState or IdleState when _hasTargetableEnemy && closestUnit.Value < range:
                    RequestAttackState();
                    break;
            }

            _stateMachine.Update();
        }

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
        
        public bool GetTarget(out NetworkedUnitBehaviour closestUnit)
        {
            closestUnit = _closestUnit;
            return _hasTargetableEnemy;
        }

        public bool HasTarget() => _hasTargetableEnemy;

        private void RequestIdleState()
        {
            _stateMachine.ChangeState(new IdleState());
        }

        private void RequestAttackState()
        {
            _stateMachine.ChangeState(new AttackState(_battleActions));
        }

        private void RequestMovementState()
        {
            _stateMachine.ChangeState(new MovementState(_battleActions));
        }
        
        public void RequestDeathState()
        {
            _stateMachine.ChangeState(new DeathState(this));
        }
    }
}
