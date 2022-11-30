using DataStructures.StateLogic;
using Features.Battle;
using Features.Unit.Modding;
using UnityEngine;

namespace Features.Unit.Battle
{
    [RequireComponent(typeof(NetworkedUnitBehaviour))]
    public class BattleBehaviour : MonoBehaviour
    {
        [SerializeField] private BattleManager_SO battleManager;
        [SerializeField] private BattleActions_SO battleActions;
        [SerializeField] private float range;
        
        private NetworkedUnitBehaviour _networkedUnitBehaviour;
        private StateMachine _stateMachine;

        private void Awake()
        {
            _stateMachine = new StateMachine();
            _stateMachine.Initialize(new IdleState());
            _networkedUnitBehaviour = GetComponent<NetworkedUnitBehaviour>();
        }

        private void Update()
        {
            switch (_stateMachine.CurrentState)
            {
                case MovementState when battleManager.EnemyUnitRuntimeSet.IsInRangeByWorldPosition(range, transform.position):
                    EnterAttackState();
                    break;
                case AttackState when !battleManager.EnemyUnitRuntimeSet.IsInRangeByWorldPosition(range, transform.position):
                    EnterMovementState();
                    break;
            }

            _stateMachine.Update();
        }

        public void EnterIdleState()
        {
            _stateMachine.ChangeState(new IdleState());
        }

        public void EnterAttackState()
        {
            _stateMachine.ChangeState(new AttackState(_networkedUnitBehaviour, battleManager.EnemyUnitRuntimeSet, battleActions));
        }

        public void EnterMovementState()
        {
            _stateMachine.ChangeState(new MovementState(_networkedUnitBehaviour, battleManager.EnemyUnitRuntimeSet, battleActions));
        }
    }
}
