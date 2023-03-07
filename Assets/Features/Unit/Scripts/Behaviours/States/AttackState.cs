using DataStructures.StateLogic;
using Features.Unit.Scripts.Class;

namespace Features.Unit.Scripts.Behaviours.Battle
{
    public class AttackState : IState
    {
        private readonly UnitBattleBehaviour _battleBehaviour;
        private readonly BattleClass _battleClass;
        
        
        public AttackState(UnitBattleBehaviour battleBehaviour, BattleClass battleClass)
        {
            _battleBehaviour = battleBehaviour;
            _battleClass = battleClass;
        }
    
        public void Enter()
        {
            _battleClass.InitializeBattleActions();
        }

        public void Execute()
        {
            if (!_battleBehaviour.TryRequestMovementStateByClosestUnit() && !_battleBehaviour.TryRequestIdleState())
            {
                _battleClass.UpdateBattleActions();
            }
        }

        public void Exit()
        {
        }
    }
}
