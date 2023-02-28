using System.Collections.Generic;
using Features.Battle.Scripts;
using Features.Battle.StateMachine;
using Features.Unit.Scripts.Stats;
using UnityEngine;

namespace Features.Unit.Scripts.Behaviours.Battle.BattleBehaviour
{
    public class ActiveBattleBehaviour : IBattleBehaviour
    {
        private readonly BattleData_SO _battleData;
        private readonly UnitServiceProvider _unitServiceProvider;
        private readonly NetworkedBattleBehaviour _networkedBattleBehaviour;
        
        public KeyValuePair<UnitServiceProvider, float> GetTarget => _closestUnit;
        private bool HasTarget { get; set; }
        private bool TargetInRange => _closestUnit.Value < _unitServiceProvider.GetService<NetworkedStatsBehaviour>().GetFinalStat(StatType.Range);
        
        
        private KeyValuePair<UnitServiceProvider, float> _closestUnit;


        public ActiveBattleBehaviour(BattleData_SO battleData, UnitServiceProvider unitServiceProvider)
        {
            _battleData = battleData;
            _unitServiceProvider = unitServiceProvider;
            _networkedBattleBehaviour = _unitServiceProvider.GetService<NetworkedBattleBehaviour>();
        }

        public void Update()
        {
            if (!_battleData.StateIsValid(typeof(BattleState), StateProgressType.Execute)) return;

            List<UnitServiceProvider> enemyUnits = _battleData.AllUnitsRuntimeSet.GetUnitsByTag(_unitServiceProvider.OpponentTagType);
            HasTarget = TryGetClosestTargetableByWorldPosition(enemyUnits, _unitServiceProvider.transform.position, out _closestUnit);

            _networkedBattleBehaviour.StateMachine.Update();
        }

        public void OnStageEnd()
        {
            if (_networkedBattleBehaviour.CurrentState is AttackState or DeathState)
            {
                ForceIdleState();
            }

            _unitServiceProvider.GetService<NetworkedBattleBehaviour>().BattleClass.OnStageEnd();
        }

        public void ForceIdleState()
        {
            _networkedBattleBehaviour.StateMachine.ChangeState(new IdleState(_networkedBattleBehaviour));
        }

        public void ForceBenchedState()
        {
            _networkedBattleBehaviour.StateMachine.ChangeState(new BenchedState(_networkedBattleBehaviour));
        }

        public bool TryRequestIdleState()
        {
            bool result = !HasTarget && _networkedBattleBehaviour.CurrentState is not DeathState;
            
            if (result)
            {
                _networkedBattleBehaviour.StateMachine.ChangeState(new IdleState(_networkedBattleBehaviour));
            }

            return result;
        }

        public bool TryRequestAttackState()
        {
            bool result = HasTarget && TargetInRange && _networkedBattleBehaviour.CurrentState is not BenchedState && 
                          _networkedBattleBehaviour.CurrentState is IdleState && _battleData.StateIsValid(typeof(BattleState), StateProgressType.Execute);
         
            if (result)
            {
                NetworkedBattleBehaviour unitBattleBehaviour = _unitServiceProvider.GetService<NetworkedBattleBehaviour>();
                _networkedBattleBehaviour.StateMachine.ChangeState(new AttackState(unitBattleBehaviour, unitBattleBehaviour.BattleClass));
            }

            return result;
        }

        public bool TryRequestMovementStateByClosestUnit()
        {
            bool result = HasTarget && !TargetInRange && _networkedBattleBehaviour.CurrentState is IdleState or AttackState && 
                          _unitServiceProvider.GetService<NetworkedStatsBehaviour>().GetFinalStat(StatType.MovementSpeed) > 0;

            if (result)
            {
                UnitServiceProvider closestStats = GetTarget.Key;
                Vector3Int enemyPosition = _battleData.TileRuntimeDictionary.GetWorldToCellPosition(closestStats.transform.position);
                //TODO: magic number
                _networkedBattleBehaviour.StateMachine.ChangeState(new MovementState(_unitServiceProvider, enemyPosition, 1, _battleData.TileRuntimeDictionary));
            }

            return result;
        }

        public bool TryRequestDeathState()
        {
            bool result = _battleData.StateIsValid(typeof(BattleState), StateProgressType.Execute);
            
            if (result)
            {
                _networkedBattleBehaviour.StateMachine.ChangeState(new DeathState(_networkedBattleBehaviour));
            }
            else
            {
                Debug.LogWarning("Requesting Death is only possible during Battle!");
            }

            return false;
        }
    
        private bool TryGetClosestTargetableByWorldPosition(List<UnitServiceProvider> networkedUnitBehaviours, Vector3 worldPosition, 
            out KeyValuePair<UnitServiceProvider, float> closestUnit)
        {
            if (!ContainsTargetable(ref networkedUnitBehaviours))
            {
                closestUnit = default;
                return false;
            }

            //get closest
            int closestUnitIndex = 0;
            float closestDistance = Vector3.Distance(worldPosition, networkedUnitBehaviours[0].transform.position);
            
            for (int index = 1; index < networkedUnitBehaviours.Count; index++)
            {
                float distanceNext = Vector3.Distance(worldPosition, networkedUnitBehaviours[index].transform.position);
                if (distanceNext < closestDistance)
                {
                    closestUnitIndex = index;
                    closestDistance = distanceNext;
                }
            }

            closestUnit = new KeyValuePair<UnitServiceProvider, float>(networkedUnitBehaviours[closestUnitIndex], closestDistance);
            return true;
        }
        
        private bool ContainsTargetable(ref List<UnitServiceProvider> networkedUnitBehaviours)
        {
            networkedUnitBehaviours.RemoveAll(e =>
            {
                NetworkedBattleBehaviour unitBattleBehaviour = e.GetService<NetworkedBattleBehaviour>();
                return !e.IsTargetable || unitBattleBehaviour.CurrentState is DeathState || unitBattleBehaviour.CurrentState is BenchedState;
            });

            return networkedUnitBehaviours.Count > 0;
        }
    }
}
