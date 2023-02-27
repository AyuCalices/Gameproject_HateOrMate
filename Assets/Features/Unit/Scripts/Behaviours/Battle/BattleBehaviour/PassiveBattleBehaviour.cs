using System.Collections;
using System.Collections.Generic;
using DataStructures.StateLogic;
using Features.Battle.Scripts;
using Features.Battle.StateMachine;
using Features.Unit.Scripts.Behaviours.Battle;
using UnityEngine;

public class PassiveBattleBehaviour : IBattleBehaviour
{
    private readonly StateMachine _stateMachine;
    private readonly BattleData_SO _battleData;
    private readonly NetworkedBattleBehaviour _networkedBattleBehaviour;

    public StateMachine StateMachine => _stateMachine;
    
    private IState CurrentState => _stateMachine.CurrentState;
    
    public PassiveBattleBehaviour(BattleData_SO battleData, NetworkedBattleBehaviour networkedBattleBehaviour)
    {
        _battleData = battleData;
        _networkedBattleBehaviour = networkedBattleBehaviour;

        _stateMachine = new StateMachine();
        _stateMachine.Initialize(new IdleState(_networkedBattleBehaviour));
    }

    public void OnStageEnd()
    {
        if (CurrentState is DeathState)
        {
            ForceIdleState();
        }
    }

    public void Update() { }

    public void ForceIdleState()
    {
        _stateMachine.ChangeState(new IdleState(_networkedBattleBehaviour));
    }

    public void ForceBenchedState()
    {
        _stateMachine.ChangeState(new BenchedState(_networkedBattleBehaviour));
    }

    public bool TryRequestIdleState()
    {
        return false;
    }

    public bool TryRequestAttackState()
    {
        return false;
    }

    public bool TryRequestMovementStateByClosestUnit()
    {
        return false;
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
