using DataStructures.StateLogic;
using Features.GlobalReferences;
using Features.Unit.Modding;

namespace Features.Unit.Battle
{
    public class MovementState : IState
    {
        private readonly NetworkedUnitBehaviour _ownerNetworkedUnitBehaviour;
        private readonly NetworkedUnitRuntimeSet_SO _opponentNetworkedUnitRuntimeSet;
        private readonly BattleActions_SO _battleActions;
        
        public MovementState(NetworkedUnitBehaviour ownerNetworkedUnitBehaviour, NetworkedUnitRuntimeSet_SO opponentNetworkedUnitRuntimeSet, BattleActions_SO battleActions)
        {
            _ownerNetworkedUnitBehaviour = ownerNetworkedUnitBehaviour;
            _opponentNetworkedUnitRuntimeSet = opponentNetworkedUnitRuntimeSet;
            _battleActions = battleActions;
        }
    
        public void Enter()
        {
        }

        public void Execute()
        {
            _battleActions.Move(_ownerNetworkedUnitBehaviour, _opponentNetworkedUnitRuntimeSet);
        }

        public void Exit()
        {
        }
    }
}
