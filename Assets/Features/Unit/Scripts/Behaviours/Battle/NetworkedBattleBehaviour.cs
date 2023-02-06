using System;
using DataStructures.StateLogic;
using Features.Battle.Scripts;
using Features.Battle.StateMachine;
using Features.Unit.Scripts.Behaviours.Stat;
using Features.Unit.Scripts.View;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;


public enum TeamTagType {Own, Mate, Enemy}
namespace Features.Unit.Scripts.Behaviours.Battle
{
    [RequireComponent(typeof(NetworkedStatsBehaviour), typeof(UnitBattleView))]
    public class NetworkedBattleBehaviour : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] protected BattleData_SO battleData;
        [SerializeField] protected SpriteRenderer unitSprite;

        protected StateMachine stateMachine;
        
        public TeamTagType[] TeamTagTypes { get; private set; }
        public TeamTagType[] OpponentTagType { get; private set; }
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

        //TODO: move else
        public void SetSprite(Sprite sprite)
        {
            unitSprite.sprite = sprite;
        }

        //TODO: move else
        public void SetTeamTagType(TeamTagType[] teamTagType, TeamTagType[] opponentTagType)
        {
            TeamTagTypes = teamTagType;
            OpponentTagType = opponentTagType;
            AddRuntimeSets();
        }

        private void AddRuntimeSets()
        {
            battleData.AllUnitsRuntimeSet.Add(this);
        }
        
        private void ClearRuntimeSets()
        {
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
            bool result = battleData.StateIsValid(typeof(BattleState), StateProgressType.Execute);
            
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

        private void OnMouseDown()
        {
            Debug.Log(NetworkedStatsBehaviour.NetworkedStatServiceLocator.GetTotalValue_CheckMin(StatType.Health));
        }
    }
}
