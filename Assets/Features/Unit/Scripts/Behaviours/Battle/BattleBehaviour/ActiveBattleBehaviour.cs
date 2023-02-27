using System.Collections.Generic;
using DataStructures.StateLogic;
using Features.Battle.Scripts;
using Features.Battle.Scripts.Unit.ServiceLocatorSystem;
using Features.Battle.StateMachine;
using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Stats;
using UnityEngine;

public class ActiveBattleBehaviour : IBattleBehaviour
{
    private readonly StateMachine _stateMachine;
    private readonly BattleData_SO _battleData;
    private readonly UnitServiceProvider _unitServiceProvider;

    private KeyValuePair<UnitServiceProvider, float> _closestUnit;

    public StateMachine StateMachine => _stateMachine;
    
    public KeyValuePair<UnitServiceProvider, float> GetTarget => _closestUnit;
    private IState CurrentState => _stateMachine.CurrentState;
    private bool HasTarget { get; set; }
    private bool TargetInRange => _closestUnit.Value < _unitServiceProvider.GetService<NetworkedStatsBehaviour>().GetFinalStat(StatType.Range);


    public ActiveBattleBehaviour(BattleData_SO battleData, UnitServiceProvider unitServiceProvider)
    {
        _battleData = battleData;
        _unitServiceProvider = unitServiceProvider;

        _stateMachine = new StateMachine();
        _stateMachine.Initialize(new IdleState(_unitServiceProvider.GetService<NetworkedBattleBehaviour>()));
    }

    public void Update()
    {
        if (!_battleData.StateIsValid(typeof(BattleState), StateProgressType.Execute)) return;

        List<UnitServiceProvider> enemyUnits = _battleData.AllUnitsRuntimeSet.GetUnitsByTag(_unitServiceProvider.GetService<NetworkedBattleBehaviour>().OpponentTagType);
        HasTarget = TryGetClosestTargetableByWorldPosition(enemyUnits, _unitServiceProvider.transform.position, out _closestUnit);

        _stateMachine.Update();
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
            return !unitBattleBehaviour.IsTargetable || unitBattleBehaviour.CurrentState is DeathState || unitBattleBehaviour.CurrentState is BenchedState;
        });

        return networkedUnitBehaviours.Count > 0;
    }
    
    public void OnStageEnd()
    {
        if (CurrentState is AttackState or DeathState)
        {
            ForceIdleState();
        }

        _unitServiceProvider.GetService<NetworkedBattleBehaviour>().BattleClass.OnStageEnd();
    }

    public void ForceIdleState()
    {
        _stateMachine.ChangeState(new IdleState(_unitServiceProvider.GetService<NetworkedBattleBehaviour>()));
    }

    public void ForceBenchedState()
    {
        _stateMachine.ChangeState(new BenchedState(_unitServiceProvider.GetService<NetworkedBattleBehaviour>()));
    }

    public bool TryRequestIdleState()
    {
        bool result = !HasTarget && CurrentState is not DeathState;
            
        if (result)
        {
            _stateMachine.ChangeState(new IdleState(_unitServiceProvider.GetService<NetworkedBattleBehaviour>()));
        }

        return result;
    }

    public bool TryRequestAttackState()
    {
        bool result = HasTarget && TargetInRange && CurrentState is not BenchedState && CurrentState is IdleState && 
                      _battleData.StateIsValid(typeof(BattleState), StateProgressType.Execute);
         
        if (result)
        {
            NetworkedBattleBehaviour unitBattleBehaviour = _unitServiceProvider.GetService<NetworkedBattleBehaviour>();
            _stateMachine.ChangeState(new AttackState(unitBattleBehaviour, unitBattleBehaviour.BattleClass));
        }

        return result;
    }

    public bool TryRequestMovementStateByClosestUnit()
    {
        bool result = HasTarget && !TargetInRange && CurrentState is IdleState or AttackState && _unitServiceProvider.GetService<NetworkedBattleBehaviour>().MovementSpeed > 0;

        if (result)
        {
            UnitServiceProvider closestStats = GetTarget.Key;
            Vector3Int enemyPosition = _battleData.TileRuntimeDictionary.GetWorldToCellPosition(closestStats.transform.position);
            //TODO: magic number
            _stateMachine.ChangeState(new MovementState( _unitServiceProvider.GetService<UnitServiceProvider>(), enemyPosition, 1, _battleData.TileRuntimeDictionary));
        }

        return result;
    }

    public bool TryRequestDeathState()
    {
        bool result = _battleData.StateIsValid(typeof(BattleState), StateProgressType.Execute);
            
        if (result)
        {
            _stateMachine.ChangeState(new DeathState(_unitServiceProvider.GetService<NetworkedBattleBehaviour>()));
        }
        else
        {
            Debug.LogWarning("Requesting Death is only possible during Battle!");
        }

        return false;
    }
}
