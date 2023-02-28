using DataStructures.StateLogic;
using Features.Battle.Scripts;
using Features.Tiles.Scripts;
using Features.Unit.Scripts.Behaviours.Battle.BattleBehaviour;
using Features.Unit.Scripts.Class;
using Features.Unit.Scripts.Stats;
using Features.Unit.Scripts.View;
using UnityEngine;
using IState = DataStructures.StateLogic.IState;

namespace Features.Unit.Scripts.Behaviours.Battle
{
    public enum TeamTagType {Own, Mate, AI}

    [RequireComponent(typeof(NetworkedStatsBehaviour), typeof(UnitBattleView))]
    public class NetworkedBattleBehaviour : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] public BattleData_SO battleData;
        
        public UnitServiceProvider UnitServiceProvider { get; private set; }
        public BattleClass BattleClass { get; private set; }
        public IBattleBehaviour BattleBehaviour { get; private set; }
        public StateMachine StateMachine { get; private set; }
        
        public IState CurrentState => StateMachine.CurrentState;
        

        public void Initialize(UnitServiceProvider unitServiceProvider, BattleClass battleClass, IBattleBehaviour battleBehaviour, IState entryState)
        {
            UnitServiceProvider = unitServiceProvider;
            BattleClass = battleClass;
            BattleBehaviour = battleBehaviour;
            
            StateMachine = new StateMachine();
            StateMachine.Initialize(entryState);
        }

        private void OnDestroy()
        {
            Vector3Int gridPosition = battleData.TileRuntimeDictionary.GetWorldToCellPosition(transform.position);
            if (battleData.TileRuntimeDictionary.TryGetByGridPosition(gridPosition, out RuntimeTile tileBehaviour))
            {
                tileBehaviour.RemoveUnit();
            }
        }

        public void SetBattleClass(BattleClass battleClass, IState entryState)
        {
            BattleClass = battleClass;
            StateMachine.ChangeState(entryState);
        }
        
        public void SetBattleBehaviour(IBattleBehaviour battleBehaviour, IState entryState)
        {
            BattleBehaviour = battleBehaviour;
            StateMachine.ChangeState(entryState);
        }

        private void Update()
        {
            //TODO: go over all states and check if inject service provider is better then battlebehaviour
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
    }
}