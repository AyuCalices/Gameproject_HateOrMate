using System;
using DataStructures.StateLogic;
using Features.Battle.Scripts;
using Features.Unit.Modding;
using Features.Unit.View;
using Photon.Pun;
using UnityEngine;

namespace Features.Unit.Battle.Scripts
{
    [RequireComponent(typeof(NetworkedStatsBehaviour), typeof(UnitView))]
    public class NetworkedBattleBehaviour : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] protected BattleData_SO battleData;

        protected StateMachine stateMachine;
        
        public UnitTeamData_SO UnitTeamData { get; set; }
        public NetworkedStatsBehaviour NetworkedStatsBehaviour { get; private set; }
        public IState CurrentState => stateMachine.CurrentState;
        
        public PhotonView PhotonView { get; private set; }
        
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
            
            PhotonView = GetComponent<PhotonView>();
            unitView = GetComponent<UnitView>();
            NetworkedStatsBehaviour = GetComponent<NetworkedStatsBehaviour>();
        }

        private void Start()
        {
            //needs to be done when instantiation is completed (the UnitTeamData is not available before)
            if (UnitTeamData.ownerNetworkedPlayerUnits != null)
            {
                UnitTeamData.ownerNetworkedPlayerUnits.Add(this);
            }
            UnitTeamData.ownTeamRuntimeSet.Add(this);
            battleData.AllUnitsRuntimeSet.Add(this);
        }

        private void OnDestroy()
        {
            if (UnitTeamData.ownerNetworkedPlayerUnits != null)
            {
                UnitTeamData.ownerNetworkedPlayerUnits.Remove(this);
            }

            UnitTeamData.ownTeamRuntimeSet.Remove(this);
            
            battleData.AllUnitsRuntimeSet.Remove(this);
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
