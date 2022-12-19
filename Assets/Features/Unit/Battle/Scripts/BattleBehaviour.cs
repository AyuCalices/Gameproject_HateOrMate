using System.Collections.Generic;
using Features.Battle.Scripts;
using Features.Mod.Action;
using Features.Unit.Battle.Scripts.Actions;
using Features.Unit.Modding;
using Features.Unit.View;
using UnityEngine;

//TODO: if refactoring: needs swap between idle & death state
namespace Features.Unit.Battle.Scripts
{
    [RequireComponent(typeof(NetworkedUnitBehaviour), typeof(UnitView))]
    public class BattleBehaviour : NetworkedBattleBehaviour
    {
        [Header("Balancing")]
        //TODO: dependency injection & maybe IIsTargetable, ICanAttack
        [SerializeField] private BattleActionGenerator_SO battleActionsGenerator;
        [SerializeField] private float range;
        //TODO: check for units that cant walk
        [SerializeField] private float movementSpeed;
        
        private BattleActions _battleActions;

        public BattleActions BattleActions => _battleActions;
        

        private KeyValuePair<NetworkedUnitBehaviour, float> _closestUnit;
        
        public KeyValuePair<NetworkedUnitBehaviour, float> GetTarget => _closestUnit;
        private bool HasTarget { get; set; }
        private bool TargetInRange => _closestUnit.Value < range;
        public float MovementSpeed => movementSpeed;

        public override void OnNetworkingEnabled()
        {
            base.OnNetworkingEnabled();
            _battleActions = battleActionsGenerator.Generate(NetworkedUnitBehaviour, this, unitView);
        }

        public override void OnStageEnd()
        {
            base.OnStageEnd();
            
            if (CurrentState is AttackState)
            {
                ForceIdleState();
            }
            
            _battleActions.OnStageEnd();
        }

        private void Update()
        {
            if (battleData.CurrentState is not BattleState) return;
            
            HasTarget = NetworkedUnitBehaviour.EnemyRuntimeSet.TryGetClosestTargetableByWorldPosition(transform.position,
                    out _closestUnit);

            stateMachine.Update();
        }

        #region Request States

        internal bool TryRequestIdleState()
        {
            bool result = !HasTarget && CurrentState is not DeathState;
            
            if (result)
            {
                stateMachine.ChangeState(new IdleState(this));
            }

            return result;
        }

        internal override bool TryRequestAttackState()
        {
            bool result = HasTarget && TargetInRange && CurrentState is not DeathState && battleData.CurrentState is BattleState;
            
            if (result)
            {
                stateMachine.ChangeState(new AttackState(this, _battleActions));
            }

            return result;
        }

        internal override bool TryRequestMovementStateByClosestUnit()
        {
            bool result = HasTarget && !TargetInRange;

            if (result)
            {
                NetworkedUnitBehaviour closestUnit = GetTarget.Key;
                Vector3Int enemyPosition = battleData.TileRuntimeDictionary.GetWorldToCellPosition(closestUnit.transform.position);
                TryRequestMovementState(enemyPosition, 1);
            }

            return result;
        }
        
        internal bool TryRequestMovementState(Vector3Int targetPosition, int skipLastMovementCount)
        {
            bool result = CurrentState is not DeathState && CurrentState is not MovementState && NetworkedUnitBehaviour is LocalUnitBehaviour;
            
            if (result)
            {
                stateMachine.ChangeState(new MovementState( this, targetPosition, skipLastMovementCount));
            }

            return result;
        }
        
        internal override bool TryRequestDeathState()
        {
            bool result = battleData.CurrentState is BattleState;
            
            if (result)
            {
                stateMachine.ChangeState(new DeathState(this));
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
