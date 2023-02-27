using System;
using System.Linq;
using Features.Battle.Scripts;
using Features.Battle.Scripts.Unit.ServiceLocatorSystem;
using Features.Tiles.Scripts;
using Features.Unit.Scripts.Class;
using Features.Unit.Scripts.Stats;
using Features.Unit.Scripts.View;
using Photon.Pun;
using UnityEngine;
using IState = DataStructures.StateLogic.IState;

namespace Features.Unit.Scripts.Behaviours.Battle
{
    public enum TeamTagType {Own, Mate, AI}

    [RequireComponent(typeof(NetworkedStatsBehaviour), typeof(UnitBattleView))]
    public class NetworkedBattleBehaviour : MonoBehaviour, IPunInstantiateMagicCallback
    {
        [Header("References")]
        [SerializeField] public BattleData_SO battleData;
        [SerializeField] protected SpriteRenderer unitSprite;
        
        public UnitServiceProvider UnitServiceProvider { get; private set; }
        public IBattleBehaviour BattleBehaviour { get; private set; }   //reset state when change
        public IState CurrentState => BattleBehaviour.StateMachine.CurrentState;
        public float MovementSpeed => UnitServiceProvider.GetService<NetworkedStatsBehaviour>().GetFinalStat(StatType.MovementSpeed);
        
        private BattleClass _battleClass;
        public BattleClass BattleClass => _battleClass;

        private UnitClassData_SO _unitClassData;
        public UnitClassData_SO UnitClassData
        {
            get => _unitClassData;
            set
            {
                _unitClassData = value;
                _battleClass = UnitClassData.battleClasses.Generate(UnitServiceProvider, value.baseDamageAnimationBehaviour);
            }
        }
        
        
        public TeamTagType[] TeamTagTypes { get; private set; }
        public TeamTagType[] OpponentTagType { get; private set; }


        private bool _isTargetable;
        public bool IsTargetable
        {
            get => _isTargetable;
            private set
            {
                _isTargetable = value;
                UnitServiceProvider.GetService<UnitBattleView>().SetHealthActive(IsTargetable);
            }
        }

        private void SetTeamTagType(TeamTagType[] teamTagType, TeamTagType[] opponentTagType)
        {
            TeamTagTypes = teamTagType;
            OpponentTagType = opponentTagType;

            UnitServiceProvider.GetService<UnitDragPlacementBehaviour>().enabled = teamTagType.Contains(TeamTagType.Own);

            if (teamTagType.Contains(TeamTagType.Own) || (PhotonNetwork.IsMasterClient && teamTagType.Contains(TeamTagType.AI)))
            {
                BattleBehaviour = new ActiveBattleBehaviour(battleData, UnitServiceProvider);
            }
            else
            {
                BattleBehaviour = new PassiveBattleBehaviour(battleData, this);
            }
        }

        private void AddRuntimeSets()
        {
            battleData.AllUnitsRuntimeSet.Add(UnitServiceProvider);
        }
        
        private void ClearRuntimeSets()
        {
            battleData.AllUnitsRuntimeSet.Remove(UnitServiceProvider);
        }
    
        protected virtual void Awake()
        {
            UnitServiceProvider = GetComponent<UnitServiceProvider>();
        }

        private void OnDestroy()
        {
            Vector3Int gridPosition = battleData.TileRuntimeDictionary.GetWorldToCellPosition(transform.position);
            if (battleData.TileRuntimeDictionary.TryGetByGridPosition(gridPosition, out RuntimeTile tileBehaviour))
            {
                tileBehaviour.RemoveUnit();
            }
            
            ClearRuntimeSets();
        }

        private void Update()
        {
            BattleBehaviour?.Update();
        }

        public void OnStageEnd()
        {
            BattleBehaviour.OnStageEnd();
        }

        internal void ForceIdleState()
        {
            BattleBehaviour.ForceIdleState();
        }
        
        internal void ForceBenchedState()
        {
            BattleBehaviour.ForceBenchedState();
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
            Debug.Log(UnitServiceProvider.GetService<NetworkedStatsBehaviour>().GetFinalStat(StatType.Health));
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            object[] instantiationData = info.photonView.InstantiationData;
            
            UnitClassData_SO unitClassData = (UnitClassData_SO)instantiationData[0];
            int level = (int)instantiationData[1];
            TeamTagType[] ownerTeamTagTypes = Array.ConvertAll((int[]) instantiationData[2], value => (TeamTagType) value);
            TeamTagType[] ownerMateTagTypes = Array.ConvertAll((int[]) instantiationData[3], value => (TeamTagType) value);
            TeamTagType[] opponentTagTypes = Array.ConvertAll((int[]) instantiationData[4], value => (TeamTagType) value);
            bool isTargetable = (bool) instantiationData[5];
            bool isBenched = (bool) instantiationData[6];
            
            TeamTagType[] teamTagType = UnitServiceProvider.GetService<PhotonView>().IsMine ? ownerTeamTagTypes : ownerMateTagTypes;

            SetTeamTagType(teamTagType, opponentTagTypes);
            
            if (isBenched)
            {
                ForceBenchedState();
            }
            else
            {
                Vector3Int gridPosition = battleData.TileRuntimeDictionary.GetWorldToCellPosition(transform.position);
                if (battleData.TileRuntimeDictionary.TryGetByGridPosition(gridPosition, out RuntimeTile tileBehaviour))
                {
                    tileBehaviour.AddUnit(gameObject);
                }
            }
            
            unitSprite.sprite = unitClassData.sprite;
            IsTargetable = isTargetable;
            UnitClassData = unitClassData;
            UnitServiceProvider.GetService<NetworkedStatsBehaviour>().SetBaseStats(unitClassData.baseStatsData, level);

            AddRuntimeSets();
        }
    }
}