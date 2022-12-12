using System.Collections.Generic;
using DataStructures.StateLogic;
using ExitGames.Client.Photon;
using Features.Battle.Scripts;
using Features.Mod.Action;
using Features.Tiles;
using Features.Unit.Battle.Scripts.Actions;
using Features.Unit.Battle.Scripts.CanMoveAction;
using Features.Unit.Modding;
using Features.Unit.View;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

//TODO: if refactoring: needs swap between idle & death state
namespace Features.Unit.Battle.Scripts
{
    [RequireComponent(typeof(NetworkedUnitBehaviour), typeof(PhotonView), typeof(UnitView))]
    public class BattleBehaviour : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        [SerializeField] private IsMovable_SO isMovable;
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
            if (CurrentState is DeathState or AttackState)
            {
                _stateMachine.ChangeState(new IdleState(this));
            }
            
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
            isMovable.OnEvent(this, photonEvent);
        }

        #region Request States

        internal void ForceIdleState()
        {
            _stateMachine.ChangeState(new IdleState(this));
        }

        internal bool TryRequestIdleState()
        {
            bool result = !HasTarget && CurrentState is not DeathState;
            
            if (result)
            {
                _stateMachine.ChangeState(new IdleState(this));
            }

            return result;
        }

        internal bool TryRequestAttackState()
        {
            bool result = HasTarget && TargetInRange && CurrentState is not DeathState && battleData.CurrentState is BattleState;
            
            if (result)
            {
                _stateMachine.ChangeState(new AttackState(this, _battleActions));
            }

            return result;
        }

        internal bool TryRequestMovementStateByClosestUnit()
        {
            bool result = HasTarget && !TargetInRange;

            if (result)
            {
                NetworkedUnitBehaviour closestUnit = GetTarget.Key;
                Vector3Int enemyPosition = tileRuntimeDictionary.GetWorldToCellPosition(closestUnit.transform.position);
                TryRequestMovementState(enemyPosition, 1);
            }

            return result;
        }
        
        internal bool TryRequestMovementState(Vector3Int targetPosition, int skipLastMovementCount)
        {
            //TODO: just requesting a NetworkedUnitBehaviour is LocalUnitBehaviour is an easy fix (for preventing all clients change position - results in weird movement behaviour). Better would be to use a LocalBattleBehaviour & NetworkedUnitBehaviour. Big refactoring required!
            bool result = CurrentState is not DeathState && CurrentState is not MovementState && NetworkedUnitBehaviour is LocalUnitBehaviour;
            
            if (result)
            {
                _stateMachine.ChangeState(new MovementState(isMovable, this, targetPosition, skipLastMovementCount));
            }

            return result;
        }
        
        //TODO: when a unit dies while moving & this results in restart stage => two movement sequences start
        internal bool TryRequestDeathState()
        {
            bool result = battleData.CurrentState is BattleState;
            
            if (result)
            {
                _stateMachine.ChangeState(new DeathState(this));
            }
            else
            {
                Debug.LogWarning("Requesting Death is only possible during Battle!");
            }

            return false;
        }

        #endregion
    }
}
