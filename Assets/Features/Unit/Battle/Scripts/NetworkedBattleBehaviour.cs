using DataStructures.StateLogic;
using Features.Battle.Scripts;
using Features.Unit.Modding;
using Features.Unit.View;
using UnityEngine;

namespace Features.Unit.Battle.Scripts
{
    [RequireComponent(typeof(NetworkedStatsBehaviour), typeof(UnitView))]
    public class NetworkedBattleBehaviour : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] protected BattleData_SO battleData;

        protected StateMachine stateMachine;
        public NetworkedStatsBehaviour NetworkedStatsBehaviour { get; private set; }
        public IState CurrentState => stateMachine.CurrentState;
        
        protected UnitView unitView;
        public bool IsTargetable { get; set; }

        public virtual void OnNetworkingEnabled()
        {
            unitView.SetHealthActive(IsTargetable);
        }
    
        protected virtual void Awake()
        {
            stateMachine = new StateMachine();
            stateMachine.Initialize(new IdleState(this));
            
            unitView = GetComponent<UnitView>();
            NetworkedStatsBehaviour = GetComponent<NetworkedStatsBehaviour>();
        }

        public virtual void OnStageEnd()
        {
            if (CurrentState is DeathState)
            {
                ForceIdleState();
            }
        }

        internal void ForceIdleState()
        {
            stateMachine.ChangeState(new IdleState(this));
        }

        internal virtual bool TryRequestAttackState()
        {
            return false;
        }

        internal virtual bool TryRequestMovementStateByClosestUnit()
        {
            return false;
        }

        internal virtual bool TryRequestDeathState()
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
    }
}
