using System.Collections.Generic;
using Features.Battle.Scripts;
using Features.Unit.Battle.Scripts.Actions;
using Features.Unit.Classes;
using Features.Unit.Modding;
using Features.Unit.View;
using UnityEngine;

//TODO: if refactoring: needs swap between idle & death state
namespace Features.Unit.Battle.Scripts
{
    [RequireComponent(typeof(NetworkedStatsBehaviour), typeof(UnitView))]
    public class BattleBehaviour : NetworkedBattleBehaviour
    {
        private BattleActions _battleActions;
        public BattleActions BattleActions => _battleActions;
        public UnitClassData_SO UnitClassData { get; set; }

        private KeyValuePair<NetworkedStatsBehaviour, float> _closestUnit;
        
        public KeyValuePair<NetworkedStatsBehaviour, float> GetTarget => _closestUnit;
        private bool HasTarget { get; set; }
        private bool TargetInRange => _closestUnit.Value < UnitClassData.range;
        public float MovementSpeed => UnitClassData.movementSpeed;

        public override void OnNetworkingEnabled()
        {
            base.OnNetworkingEnabled();
            _battleActions = UnitClassData.battleActions.Generate(NetworkedStatsBehaviour, this, unitView);
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
            
            HasTarget = NetworkedStatsBehaviour.UnitTeamData.EnemyRuntimeSet.TryGetClosestTargetableByWorldPosition(transform.position,
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
                NetworkedStatsBehaviour closestStats = GetTarget.Key;
                Vector3Int enemyPosition = battleData.TileRuntimeDictionary.GetWorldToCellPosition(closestStats.transform.position);
                TryRequestMovementState(enemyPosition, 1);
            }

            return result;
        }
        
        internal bool TryRequestMovementState(Vector3Int targetPosition, int skipLastMovementCount)
        {
            bool result = CurrentState is not DeathState && CurrentState is not MovementState;
            
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
