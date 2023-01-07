using DataStructures.StateLogic;
using Features.Connection.Scripts.Utils;
using Features.Loot.Scripts.LootView;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Battle.Scripts
{
    public class LootingState : IState
    {
        private readonly BattleManager _battleManager;
        private readonly Button _continueBattleButton;
        private readonly bool _restartStage;
        private RoomDecisions<bool> _roomDecision;

        public LootingState(BattleManager battleManager, Button continueBattleButton, bool restartStage)
        {
            _battleManager = battleManager;
            _continueBattleButton = continueBattleButton;
            _restartStage = restartStage;
        }

        public void Enter()
        {
            _roomDecision = new RoomDecisions<bool>("Placement", false);
            _continueBattleButton.interactable = true;
            for (int i = 0; i < _continueBattleButton.transform.childCount; i++)
            {
                _continueBattleButton.transform.GetChild(i).gameObject.SetActive(true);
            }
            
            _continueBattleButton.onClick.AddListener(() =>
            {
                _roomDecision.SetLocalDecision(true);
                _continueBattleButton.interactable = false;
                for (int i = 0; i < _continueBattleButton.transform.childCount; i++)
                {
                    _continueBattleButton.transform.GetChild(i).gameObject.SetActive(false);
                }
            });
        }

        public void Execute()
        {
            if (_roomDecision == null) return;
            _roomDecision.UpdateDecision(() => _battleManager.RequestStageSetupState(_restartStage));
        }

        public void Exit()
        {
        }
    }
}
