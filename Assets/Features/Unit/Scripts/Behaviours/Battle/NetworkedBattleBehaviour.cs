using System;
using System.Linq;
using DataStructures.StateLogic;
using Features.Battle.Scripts;
using Features.Battle.StateMachine;
using Features.Tiles.Scripts;
using Features.Unit.Scripts.Class;
using Features.Unit.Scripts.Stats;
using Features.Unit.Scripts.View;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Features.Unit.Scripts.Behaviours.Battle
{
    public enum TeamTagType {Own, Mate, AI}

    [RequireComponent(typeof(NetworkedStatsBehaviour), typeof(UnitBattleView))]
    public class NetworkedBattleBehaviour : MonoBehaviour, IPunInstantiateMagicCallback
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
                _battleClass = UnitClassData.battleClasses.Generate(value.baseDamageAnimationBehaviour, NetworkedStatsBehaviour, this, _unitBattleView);
            }
        }
        
        
        public TeamTagType[] TeamTagTypes { get; private set; }
        public TeamTagType[] OpponentTagType { get; private set; }
        public bool IsSpawnedLocally { get; set; }
        public int SpawnerInstanceIndex { get; set; }
        
        public NetworkedStatsBehaviour NetworkedStatsBehaviour { get; private set; }
        
        
        
        public PhotonView PhotonView { get; private set; }

        private UnitBattleView _unitBattleView;

        private bool _isTargetable;
        public bool IsTargetable
        {
            get => _isTargetable;
            set
            {
                _isTargetable = value;
                _unitBattleView.SetHealthActive(IsTargetable);
            }
        }

        public void SetSprite(Sprite sprite)
        {
            unitSprite.sprite = sprite;
        }

        public void SetTeamTagType(TeamTagType[] teamTagType, TeamTagType[] opponentTagType)
        {
            TeamTagTypes = teamTagType;
            OpponentTagType = opponentTagType;
            AddRuntimeSets();

            //TODO: add movement to own Units
            if (teamTagType.Contains(TeamTagType.Own) || (PhotonNetwork.IsMasterClient && teamTagType.Contains(TeamTagType.AI)))
            {
                BattleBehaviour = new ActiveBattleBehaviour(battleData, this);
            }
            else
            {
                BattleBehaviour = new PassiveBattleBehaviour(battleData, this);
            }
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
            _unitBattleView = GetComponent<UnitBattleView>();
            NetworkedStatsBehaviour = GetComponent<NetworkedStatsBehaviour>();
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
            BattleBehaviour?.OnStageEnd();
        }

        internal void ForceIdleState()
        {
            BattleBehaviour?.ForceIdleState();
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

        public void Destroy()
        {
            Destroy(gameObject);
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            object[] instantiationData = info.photonView.InstantiationData;
            
            int ownerActorNumber = (int)instantiationData[0];
            UnitClassData_SO unitClassData = (UnitClassData_SO)instantiationData[1];
            Vector3Int gridPosition = (Vector3Int)instantiationData[2];
            int index = (int)instantiationData[3];
            int level = (int)instantiationData[4];
            TeamTagType[] ownerTeamTagTypes = Array.ConvertAll((int[]) instantiationData[5], value => (TeamTagType) value);
            TeamTagType[] ownerMateTagTypes = Array.ConvertAll((int[]) instantiationData[6], value => (TeamTagType) value);
            TeamTagType[] opponentTagTypes = Array.ConvertAll((int[]) instantiationData[7], value => (TeamTagType) value);
            bool isTargetable = (bool) instantiationData[8];
            
            bool isOwner = PhotonNetwork.LocalPlayer.ActorNumber == ownerActorNumber;
            TeamTagType[] teamTagType = isOwner ? ownerTeamTagTypes : ownerMateTagTypes;
            SetTeamTagType(teamTagType, opponentTagTypes);
            
            if (unitClassData.sprite != null)
            {
                SetSprite(unitClassData.sprite);
            }
            
            IsTargetable = isTargetable;
            SpawnerInstanceIndex = index;
            UnitClassData = unitClassData;
            NetworkedStatsBehaviour.SetBaseStats(unitClassData.baseStatsData, level);
            
            if (battleData.TileRuntimeDictionary.TryGetByGridPosition(gridPosition, out RuntimeTile tileBehaviour))
            {
                tileBehaviour.AddUnit(gameObject);
            }
        }
    }
}