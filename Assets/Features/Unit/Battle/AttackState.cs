using DataStructures.StateLogic;
using Features.GlobalReferences;
using Features.Unit.Modding;
using Features.Unit.Modding.Stat;
using UnityEngine;

namespace Features.Unit.Battle
{
    public class AttackState : IState
    {
        private readonly BattleActions _battleActions;
        
        
        public AttackState(BattleActions battleActions)
        {
            _battleActions = battleActions;
        }
    
        public void Enter()
        {
            
        }

        public void Execute()
        {
            //TODO: send event for attackTime UI updated

            _battleActions.UpdateBattleActions();
        }

        public void Exit()
        {
        }
    }
}
