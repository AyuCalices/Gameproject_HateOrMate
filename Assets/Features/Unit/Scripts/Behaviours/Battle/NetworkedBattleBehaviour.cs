using DataStructures.StateLogic;
using Features.Battle.Scripts;
using Features.Unit.Scripts.Behaviours.Stat;
using Features.Unit.Scripts.View;
using Photon.Pun;
using UnityEngine;

namespace Features.Unit.Scripts.Behaviours.Battle
{
    [RequireComponent(typeof(NetworkedStatsBehaviour), typeof(UnitBattleView))]
    public class NetworkedBattleBehaviour : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] protected BattleData_SO battleData;

        [SerializeField] protected SpriteRenderer unitSprite;

        protected StateMachine stateMachine;

        private UnitTeamData_SO _unitTeamData;

        public UnitTeamData_SO UnitTeamData
        {
            get => _unitTeamData;
            set
            {
                _unitTeamData = value;
                AddRuntimeSets();
            }
        }
        
        public bool IsSpawnedLocally { get; set; }
        
        public int SpawnerInstanceIndex { get; set; }
        
        public NetworkedStatsBehaviour NetworkedStatsBehaviour { get; private set; }
        public IState CurrentState => stateMachine.CurrentState;
        
        public PhotonView PhotonView { get; private set; }
        
        protected UnitBattleView unitBattleView;

        private bool _isTargetable;
        public bool IsTargetable
        {
            get => _isTargetable;
            set
            {
                _isTargetable = value;
                unitBattleView.SetHealthActive(IsTargetable);
            }
        }

        public void SetSprite(Sprite sprite)
        {
            unitSprite.sprite = sprite;
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
            unitBattleView = GetComponent<UnitBattleView>();
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
            bool result = battleData.CurrentState is CoroutineState;
            
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
