using System.Linq;
using DataStructures.StateLogic;
using Features.Connection.Scripts;
using Features.Connection.Scripts.Utils;
using Features.Loot.Scripts.LootView;
using Features.Unit.Scripts.Behaviours.Battle;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Battle.Scripts
{
    public class LootingState : IState
    {
        private readonly BattleManager _battleManager;
        private readonly BattleData_SO _battleData;
        private readonly ErrorPopup _errorPopup;
        private readonly Transform _instantiationParent;
        private readonly Button _continueBattleButton;
        private readonly bool _restartStage;
        private RoomDecisions<bool> _roomDecision;

        public LootingState(BattleManager battleManager, BattleData_SO battleData, ErrorPopup errorPopup, Transform instantiationParent, Button continueBattleButton, bool restartStage)
        {
            _battleManager = battleManager;
            _battleData = battleData;
            _errorPopup = errorPopup;
            _instantiationParent = instantiationParent;
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
                if (_battleData.LocalUnitRuntimeSet.GetItems().Any(networkedBattleBehaviour => !networkedBattleBehaviour.IsSpawnedLocally))
                {
                    _roomDecision.SetLocalDecision(true);
                    _continueBattleButton.interactable = false;
                    for (int i = 0; i < _continueBattleButton.transform.childCount; i++)
                    {
                        _continueBattleButton.transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
                else
                {
                    _errorPopup.Instantiate(_instantiationParent, "You must at least place one unit into the battle area!");
                }
            });
        }

        public void Execute()
        {
            if (_roomDecision == null) return;
            _roomDecision.IsValidDecision(() => _battleManager.RequestStageSetupState(_restartStage));
        }

        public void Exit()
        {
        }
    }
}
