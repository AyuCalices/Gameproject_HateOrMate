using DataStructures.StateLogic;
using Features.Loot;
using UnityEngine.UI;

namespace Features.Battle
{
    public class LootingState : IState
    {
        private readonly BattleManager _battleManager;
        private readonly LootSelectionBehaviour _lootSelectionBehaviour;
        private readonly Button _continueBattleButton;
        private RoomDecisions<bool> _roomDecision;

        public LootingState(BattleManager battleManager, LootSelectionBehaviour lootSelectionBehaviour, Button continueBattleButton)
        {
            _battleManager = battleManager;
            _lootSelectionBehaviour = lootSelectionBehaviour;
            _continueBattleButton = continueBattleButton;
        }

        public void Enter()
        {
            _roomDecision = new RoomDecisions<bool>("Placement");
            _battleManager.DisableLootPhaseRequested();
            _lootSelectionBehaviour.gameObject.SetActive(true);
            _continueBattleButton.gameObject.SetActive(true);
            _continueBattleButton.onClick.AddListener(() => _roomDecision.SetLocalDecision(true));
        }

        public void Execute()
        {
            _roomDecision.UpdateDecision(() => _battleManager.RequestBattleState());
        }

        public void Exit()
        {
            _continueBattleButton.gameObject.SetActive(false);
        }
    }
}
