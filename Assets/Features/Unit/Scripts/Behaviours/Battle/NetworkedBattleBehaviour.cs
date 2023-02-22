using System;
using DataStructures.StateLogic;
using Features.Battle.Scripts;
using Features.Battle.StateMachine;
using Features.Unit.Scripts.Class;
using Features.Unit.Scripts.Stats;
using Features.Unit.Scripts.View;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Features.Unit.Scripts.Behaviours.Battle
{
    public enum TeamTagType {Own, Mate, Enemy}

    [RequireComponent(typeof(NetworkedStatsBehaviour), typeof(UnitBattleView))]
    public class NetworkedBattleBehaviour : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] protected BattleData_SO battleData;
        [SerializeField] protected SpriteRenderer unitSprite;
        
        
        public IBattleBehaviour BattleBehaviour { get; set; }
        public IState CurrentState => BattleBehaviour.StateMachine.CurrentState;
        public float MovementSpeed => NetworkedStatsBehaviour.GetFinalStat(StatType.MovementSpeed);
        
        private BattleClass _battleClass;
        public BattleClass BattleClass => _battleClass;

        private UnitClassData_SO _unitClassData;
        public UnitClassData_SO UnitClassData
        {
            get => _unitClassData;
            set
            {
                _unitClassData = value;
                _battleClass = UnitClassData.battleClasses.Generate(value.baseDamageAnimationBehaviour, NetworkedStatsBehaviour, this, unitBattleView);
            }
        }
        
        
        public TeamTagType[] TeamTagTypes { get; private set; }
        public TeamTagType[] OpponentTagType { get; private set; }
        public bool IsSpawnedLocally { get; set; }
        public int SpawnerInstanceIndex { get; set; }
        
        public NetworkedStatsBehaviour NetworkedStatsBehaviour { get; private set; }
        
        
        
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
            PhotonView = GetComponent<PhotonView>();
            unitBattleView = GetComponent<UnitBattleView>();
            NetworkedStatsBehaviour = GetComponent<NetworkedStatsBehaviour>();
        }

        private void OnDestroy()
        {
            ClearRuntimeSets();
        }

        private void Update()
        {
            BattleBehaviour.Update();
        }

        public void OnStageEnd()
        {
            BattleBehaviour.OnStageEnd();
        }

        internal void ForceIdleState()
        {
            BattleBehaviour.ForceIdleState();
        }
        
        internal bool TryRequestIdleState()
        {
            return BattleBehaviour.TryRequestIdleState();
        }

        internal bool TryRequestAttackState()
        {
            return BattleBehaviour.TryRequestAttackState();
        }

        internal bool TryRequestMovementStateByClosestUnit()
        {
            return BattleBehaviour.TryRequestMovementStateByClosestUnit();
        }

        internal bool TryRequestDeathState()
        {
            return BattleBehaviour.TryRequestDeathState();
        }

        private void OnMouseDown()
        {
            Debug.Log(NetworkedStatsBehaviour.GetFinalStat(StatType.Health));
        }
    }
}