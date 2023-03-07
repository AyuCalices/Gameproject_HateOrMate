using System;
using System.Linq;
using DataStructures.Event;
using DataStructures.StateLogic;
using Features.Battle.Scripts;
using Features.Tiles.Scripts;
using Features.Unit.Scripts.Behaviours.Battle.BattleBehaviour;
using Features.Unit.Scripts.Class;
using Features.Unit.Scripts.View;
using Photon.Pun;
using UnityEngine;

namespace Features.Unit.Scripts.Behaviours.Battle
{
    public class UnitServiceProvider : MonoBehaviour, IPunInstantiateMagicCallback
    {
        [SerializeField] private BattleData_SO battleData;
        public GameEvent onHitEvent; //TODO: swap to different onHit sound system

        public bool IsTargetable { get; private set; }
        public UnitClassData_SO UnitClassData { get; private set; }
        public TeamTagType[] TeamTagTypes { get; private set; }
        public TeamTagType[] OpponentTagType { get; private set; }

        public T GetService<T>() where T : MonoBehaviour => _unitServiceController.Get<T>();
        
        
        private ServiceLocatorObject<MonoBehaviour> _unitServiceController;
        private UnitStatsBehaviour _unitStatsBehaviour;
        private UnitBattleBehaviour _unitBattleBehaviour;
        private UnitDragPlacementBehaviour _unitDragPlacementBehaviour;
        private UnitBattleView _unitBattleView;
        private PhotonView _photonView;
        

        private void Awake()
        {
            _unitStatsBehaviour = GetComponent<UnitStatsBehaviour>();
            _unitBattleBehaviour = GetComponent<UnitBattleBehaviour>();
            _unitDragPlacementBehaviour = GetComponent<UnitDragPlacementBehaviour>();
            _unitBattleView = GetComponent<UnitBattleView>();
            _photonView = GetComponent<PhotonView>();

            _unitServiceController = new ServiceLocatorObject<MonoBehaviour>();
            _unitServiceController.Register(this);
            _unitServiceController.Register(_unitStatsBehaviour);
            _unitServiceController.Register(_unitBattleBehaviour);
            _unitServiceController.Register(_unitDragPlacementBehaviour);
            _unitServiceController.Register(_unitBattleView);
            _unitServiceController.Register(_photonView);
        }
        
        private void OnDestroy()
        {
            ClearRuntimeSets();
        }
        
        public void Destroy()
        {
            Destroy(gameObject);
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            object[] instantiationData = info.photonView.InstantiationData;
            
            UnitClassData = (UnitClassData_SO)instantiationData[0];
            int level = (int)instantiationData[1];
            TeamTagType[] ownerTeamTagTypes = Array.ConvertAll((int[]) instantiationData[2], value => (TeamTagType) value);
            TeamTagType[] ownerMateTagTypes = Array.ConvertAll((int[]) instantiationData[3], value => (TeamTagType) value);
            TeamTagTypes = GetService<PhotonView>().IsMine ? ownerTeamTagTypes : ownerMateTagTypes;
            OpponentTagType = Array.ConvertAll((int[]) instantiationData[4], value => (TeamTagType) value);
            IsTargetable = (bool) instantiationData[5];
            bool isBenched = (bool) instantiationData[6];
            
            _unitDragPlacementBehaviour.Initialize(TeamTagTypes.Contains(TeamTagType.Own));
            InitializeBattleBehaviour(isBenched);
            _unitBattleView.Initialize(UnitClassData.sprite, IsTargetable, true);
            _unitStatsBehaviour.Initialize(this, UnitClassData.baseStatsData, level);
            AddRuntimeSets();
        }

        private void InitializeBattleBehaviour(bool isBenched)
        {
            IState entryState = isBenched ? new BenchedState(_unitBattleBehaviour) : new IdleState(_unitBattleBehaviour);
            IBattleBehaviour battleBehaviour = TeamTagTypes.Contains(TeamTagType.Own) || PhotonNetwork.IsMasterClient && TeamTagTypes.Contains(TeamTagType.AI)
                ? new ActiveBattleBehaviour(battleData, this)
                : new PassiveBattleBehaviour(battleData, this);
            BattleClass battleClass = UnitClassData.battleClasses.Generate(this, UnitClassData.baseDamageAnimationBehaviour);
            _unitBattleBehaviour.Initialize(this, battleClass, battleBehaviour, entryState);
            
            if (!isBenched)
            {
                Vector3Int gridPosition = battleData.TileRuntimeDictionary.GetWorldToCellPosition(transform.position);
                if (battleData.TileRuntimeDictionary.TryGetByGridPosition(gridPosition, out RuntimeTile tileBehaviour))
                {
                    tileBehaviour.AddUnit(gameObject);
                }
            }
        }
        
        private void AddRuntimeSets()
        {
            battleData.UnitsServiceProviderRuntimeSet.Add(this);
        }
        
        private void ClearRuntimeSets()
        {
            battleData.UnitsServiceProviderRuntimeSet.Remove(this);
        }
    }
}
