using DataStructures.StateLogic;
using Features.Loot;

namespace Features.Battle
{
    public class LootingState : IState
    {
        private readonly BattleManager _battleManager;
        private readonly LootSelectionBehaviour _lootSelectionBehaviour;

        public LootingState(BattleManager battleManager, LootSelectionBehaviour lootSelectionBehaviour)
        {
            _battleManager = battleManager;
            _lootSelectionBehaviour = lootSelectionBehaviour;
        }

        public void Enter()
        {
            _battleManager.DisableLootPhaseRequested();
            _lootSelectionBehaviour.gameObject.SetActive(true);
        }

        public void Execute()
        {
        
        }

        public void Exit()
        {
            _lootSelectionBehaviour.gameObject.SetActive(false);
        }
    }
}
