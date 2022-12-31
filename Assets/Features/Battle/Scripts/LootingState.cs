using DataStructures.StateLogic;
using ExitGames.Client.Photon;
using Features.Connection;
using Features.Connection.Scripts.Utils;
using Features.Loot;
using Features.Loot.Scripts;
using Features.Loot.Scripts.Generator;
using Features.Loot.Scripts.LootView;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Battle.Scripts
{
    public class LootingState : IState
    {
        private readonly BattleManager _battleManager;
        private readonly BattleData_SO _battleData;
        private readonly LootSelectionBehaviour _lootSelectionBehaviour;
        private readonly Button _continueBattleButton;
        private readonly bool _restartStage;
        private RoomDecisions<bool> _roomDecision;

        public static int LootCount { get; set; }

        public LootingState(BattleManager battleManager, BattleData_SO battleData, LootSelectionBehaviour lootSelectionBehaviour, Button continueBattleButton, bool restartStage)
        {
            _battleManager = battleManager;
            _battleData = battleData;
            _lootSelectionBehaviour = lootSelectionBehaviour;
            _continueBattleButton = continueBattleButton;
            _restartStage = restartStage;
        }

        public void Enter()
        {
            _roomDecision = new RoomDecisions<bool>("Placement", false);
            _lootSelectionBehaviour.gameObject.SetActive(true);
            _continueBattleButton.interactable = true;
            for (int i = 0; i < _continueBattleButton.transform.childCount; i++)
            {
                Debug.Log(i);
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
            
            if (!PhotonNetwork.IsMasterClient) return;

            LootableGenerator_SO[] lootables = new LootableGenerator_SO[LootCount];
            for (int index = 0; index < lootables.Length; index++)
            {
                lootables[index] = _battleData.LootTable.RandomizeLootableGenerator();
            }

            if (!_restartStage)
            {
                SendLootableByRaiseEvent(lootables);
            }
        }

        public void Execute()
        {
            if (_roomDecision == null) return;
            _roomDecision.UpdateDecision(() => _battleManager.RequestStageSetupState(_restartStage));
        }

        public void Exit()
        {
        }
        
        private void SendLootableByRaiseEvent(LootableGenerator_SO[] lootable)
        {
            object[] data = new object[]
            {
                lootable
            };
            
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.All,
                CachingOption = EventCaching.AddToRoomCache
            };

            SendOptions sendOptions = new SendOptions
            {
                Reliability = true
            };
            
            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnObtainLoot, data, raiseEventOptions, sendOptions);
        }
    }
}
