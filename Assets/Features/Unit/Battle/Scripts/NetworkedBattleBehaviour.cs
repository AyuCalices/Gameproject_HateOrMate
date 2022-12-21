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

        private UnitTeamData_SO _unitTeamData;

        public UnitTeamData_SO UnitTeamData
        {
            get => _unitTeamData;
            set
            {
                //TODO: re-apply modifier - eg: remove networked stats and send it through attack. when unit gets instantiated, all mods need to get applied to it
                if (_unitTeamData != null)
                {
                    ClearRuntimeSets();
                }
                _unitTeamData = value;
                AddRuntimeSets();
            }
        }

        public NetworkedStatsBehaviour NetworkedStatsBehaviour { get; private set; }
        public IState CurrentState => stateMachine.CurrentState;
        
        public PhotonView PhotonView { get; private set; }
        
        protected UnitView unitView;

        private bool _isTargetable;
        public bool IsTargetable
        {
            get => _isTargetable;
            set
            {
                _isTargetable = value;
                unitView.SetHealthActive(IsTargetable);
            }
        }

        private void AddRuntimeSets()
        {
            if (UnitTeamData.ownerNetworkedPlayerUnits != null)
            {
                UnitTeamData.ownerNetworkedPlayerUnits.Add(this);
            }
            UnitTeamData.ownTeamRuntimeSet.Add(this);
            battleData.AllUnitsRuntimeSet.Add(this);
        }
        
        private void ClearRuntimeSets()
        {
            if (UnitTeamData.ownerNetworkedPlayerUnits != null)
            {
                UnitTeamData.ownerNetworkedPlayerUnits.Remove(this);
            }

            UnitTeamData.ownTeamRuntimeSet.Remove(this);
            
            battleData.AllUnitsRuntimeSet.Remove(this);
        }
    
        protected virtual void Awake()
        {
            stateMachine = new StateMachine();
            stateMachine.Initialize(new IdleState(this));
            
            PhotonView = GetComponent<PhotonView>();
            unitView = GetComponent<UnitView>();
            NetworkedStatsBehaviour = GetComponent<NetworkedStatsBehaviour>();
        }

        private void OnDestroy()
        {
            ClearRuntimeSets();
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
