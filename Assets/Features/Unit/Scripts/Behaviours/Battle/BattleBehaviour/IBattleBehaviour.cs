using System.Collections;
using System.Collections.Generic;
using DataStructures.StateLogic;
using UnityEngine;

public interface IBattleBehaviour
{
    StateMachine StateMachine { get; }
    
    void OnStageEnd();

    void Update();
    
    void ForceIdleState();
    void ForceBenchedState();
    bool TryRequestIdleState();
    bool TryRequestAttackState();
    bool TryRequestMovementStateByClosestUnit();
    bool TryRequestDeathState();
}
