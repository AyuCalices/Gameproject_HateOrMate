using System.Collections.Generic;
using Features.BattleScene.Scripts;
using Features.BattleScene.Scripts.StateMachine;
using Features.BattleScene.Scripts.States;
using Features.Unit.Scripts.Behaviours.Services.UnitStats;
using Features.Unit.Scripts.Behaviours.States;
using UnityEngine;

namespace Features.Unit.Scripts.Behaviours.Services.BattleBehaviour
{
    public class ActiveBattleBehaviour : IBattleBehaviour
    {
        private readonly BattleData_SO _battleData;
        private readonly UnitServiceProvider _unitServiceProvider;
        private readonly UnitBattleBehaviour _unitBattleBehaviour;
        
        public KeyValuePair<UnitServiceProvider, float> GetTarget => _closestUnit;
        private bool HasTarget { get; set; }
        private bool TargetInRange => _closestUnit.Value < _unitServiceProvider.GetService<UnitStatsBehaviour>().GetFinalStat(StatType.Range);
        
        
        private KeyValuePair<UnitServiceProvider, float> _closestUnit;


        public ActiveBattleBehaviour(BattleData_SO battleData, UnitServiceProvider unitServiceProvider)
        {
            _battleData = battleData;
            _unitServiceProvider = unitServiceProvider;
            _unitBattleBehaviour = _unitServiceProvider.GetService<UnitBattleBehaviour>();
        }

        public void Update()
        {
            if (!_battleData.StateIsValid(typeof(BattleState), StateProgressType.Execute)) return;

            List<UnitServiceProvider> enemyUnits = _battleData.UnitsServiceProviderRuntimeSet.GetUnitsByTag(_unitServiceProvider.OpponentTagType);
            HasTarget = TryGetClosestTargetableByWorldPosition(enemyUnits, _unitServiceProvider.transform.position, out _closestUnit);

            _unitBattleBehaviour.StateMachine.Update();
        }

        public void OnStageEnd()
        {
            if (_unitBattleBehaviour.CurrentState is AttackState or DeathState)
            {
                ForceIdleState();
            }

            _unitServiceProvider.GetService<UnitBattleBehaviour>().BattleClass.OnStageEnd();
        }

        public void ForceIdleState()
        {
            _unitBattleBehaviour.StateMachine.ChangeState(new IdleState(_unitBattleBehaviour));
        }

        public void ForceBenchedState()
        {
            _unitBattleBehaviour.StateMachine.ChangeState(new BenchedState(_unitBattleBehaviour));
        }

        public bool TryRequestIdleState()
        {
            bool result = !HasTarget && _unitBattleBehaviour.CurrentState is not DeathState;
            
            if (result)
            {
                _unitBattleBehaviour.StateMachine.ChangeState(new IdleState(_unitBattleBehaviour));
            }

            return result;
        }

        public bool TryRequestAttackState()
        {
            bool result = HasTarget && TargetInRange && _unitBattleBehaviour.CurrentState is not BenchedState && 
                          _unitBattleBehaviour.CurrentState is IdleState && _battleData.StateIsValid(typeof(BattleState), StateProgressType.Execute);
         
            if (result)
            {
                UnitBattleBehaviour unitBattleBehaviour = _unitServiceProvider.GetService<UnitBattleBehaviour>();
                _unitBattleBehaviour.StateMachine.ChangeState(new AttackState(unitBattleBehaviour, unitBattleBehaviour.BattleClass));
            }

            return result;
        }

        public bool TryRequestMovementStateByClosestUnit()
        {
            bool result = HasTarget && !TargetInRange && _unitBattleBehaviour.CurrentState is IdleState or AttackState && 
                          _unitServiceProvider.GetService<UnitStatsBehaviour>().GetFinalStat(StatType.MovementSpeed) > 0;

            if (result)
            {
                UnitServiceProvider closestStats = GetTarget.Key;
                Vector3Int enemyPosition = _battleData.TileRuntimeDictionary.GetWorldToCellPosition(closestStats.transform.position);
                //TODO: magic number
                _unitBattleBehaviour.StateMachine.ChangeState(new MovementState(_unitServiceProvider, enemyPosition, 1, _battleData.TileRuntimeDictionary));
            }

            return result;
        }

        public bool TryRequestDeathState()
        {
            bool result = _battleData.StateIsValid(typeof(BattleState), StateProgressType.Execute);
            
            if (result)
            {
                _unitBattleBehaviour.StateMachine.ChangeState(new DeathState(_unitBattleBehaviour, _battleData));
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
                UnitBattleBehaviour unitBattleBehaviour = e.GetService<UnitBattleBehaviour>();
                return !e.IsTargetable || unitBattleBehaviour.CurrentState is DeathState || unitBattleBehaviour.CurrentState is BenchedState;
            });

            return networkedUnitBehaviours.Count > 0;
        }
    }
}
