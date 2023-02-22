using System.Collections.Generic;
using DataStructures.StateLogic;
using Features.Battle.Scripts;
using Features.Battle.StateMachine;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Stats;
using UnityEngine;

public class ActiveBattleBehaviour : IBattleBehaviour
{
    private readonly StateMachine _stateMachine;
    private readonly BattleData_SO _battleData;
    private readonly NetworkedBattleBehaviour _networkedBattleBehaviour;

    private KeyValuePair<NetworkedBattleBehaviour, float> _closestUnit;

    public StateMachine StateMachine => _stateMachine;
    
    public KeyValuePair<NetworkedBattleBehaviour, float> GetTarget => _closestUnit;
    private IState CurrentState => _stateMachine.CurrentState;
    private bool HasTarget { get; set; }
    private bool TargetInRange => _closestUnit.Value < _networkedBattleBehaviour.NetworkedStatsBehaviour.GetFinalStat(StatType.Range);


    public ActiveBattleBehaviour(BattleData_SO battleData, NetworkedBattleBehaviour networkedBattleBehaviour)
    {
        _battleData = battleData;
        _networkedBattleBehaviour = networkedBattleBehaviour;

        _stateMachine = new StateMachine();
        _stateMachine.Initialize(new IdleState(_networkedBattleBehaviour));
    }

    public void Update()
    {
        if (!_battleData.StateIsValid(typeof(BattleState), StateProgressType.Execute)) return;

        List<NetworkedBattleBehaviour> enemyUnits = _battleData.AllUnitsRuntimeSet.GetUnitsByTag(_networkedBattleBehaviour.OpponentTagType);
        HasTarget = TryGetClosestTargetableByWorldPosition(enemyUnits, _networkedBattleBehaviour.transform.position, out _closestUnit);

        _stateMachine.Update();
    }
    
    private bool TryGetClosestTargetableByWorldPosition(List<NetworkedBattleBehaviour> networkedUnitBehaviours, Vector3 worldPosition, 
        out KeyValuePair<NetworkedBattleBehaviour, float> closestUnit)
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

        closestUnit = new KeyValuePair<NetworkedBattleBehaviour, float>(networkedUnitBehaviours[closestUnitIndex], closestDistance);
        return true;
    }
        
    private bool ContainsTargetable(ref List<NetworkedBattleBehaviour> networkedUnitBehaviours)
    {
        networkedUnitBehaviours.RemoveAll(e => !e.IsTargetable || e.CurrentState is DeathState || e.IsSpawnedLocally);

        return networkedUnitBehaviours.Count > 0;
    }
    
    public void OnStageEnd()
    {
        if (CurrentState is AttackState or DeathState)
        {
            ForceIdleState();
        }

        _networkedBattleBehaviour.BattleClass.OnStageEnd();
    }

    public void ForceIdleState()
    {
        _stateMachine.ChangeState(new IdleState(_networkedBattleBehaviour));
    }

    public bool TryRequestIdleState()
    {
        bool result = !HasTarget && CurrentState is not DeathState;
            
        if (result)
        {
            _stateMachine.ChangeState(new IdleState(_networkedBattleBehaviour));
        }

        return result;
    }

    public bool TryRequestAttackState()
    {
        bool result = HasTarget && TargetInRange && !_networkedBattleBehaviour.IsSpawnedLocally && CurrentState is IdleState && 
                      _battleData.StateIsValid(typeof(BattleState), StateProgressType.Execute);
         
        if (result)
        {
            _stateMachine.ChangeState(new AttackState(_networkedBattleBehaviour, _networkedBattleBehaviour.BattleClass));
        }

        return result;
    }

    public bool TryRequestMovementStateByClosestUnit()
    {
        bool result = HasTarget && !TargetInRange && CurrentState is IdleState or AttackState && _networkedBattleBehaviour.MovementSpeed > 0;

        if (result)
        {
            NetworkedBattleBehaviour closestStats = GetTarget.Key;
            Vector3Int enemyPosition = _battleData.TileRuntimeDictionary.GetWorldToCellPosition(closestStats.transform.position);
            //TODO: magic number
            _stateMachine.ChangeState(new MovementState( _networkedBattleBehaviour, enemyPosition, 1, _battleData.TileRuntimeDictionary));
        }

        return result;
    }

    public bool TryRequestDeathState()
    {
        bool result = _battleData.StateIsValid(typeof(BattleState), StateProgressType.Execute);
            
        if (result)
        {
            _stateMachine.ChangeState(new DeathState(_networkedBattleBehaviour));
        }
        else
        {
            Debug.LogWarning("Requesting Death is only possible during Battle!");
        }

        return false;
    }
}
