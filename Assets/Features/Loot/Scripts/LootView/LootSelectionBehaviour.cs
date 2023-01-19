using System;
using System.Collections.Generic;
using System.Linq;
using Features.Battle.Scripts;
using Features.Connection.Scripts.Utils;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Features.Loot.Scripts.LootView
{
    public class LootSelectionBehaviour : MonoBehaviourPunCallbacks
    {
        [SerializeField] private BattleData_SO battleData;
        [SerializeField] private IntRoomDecitions_SO lootIndexRoomDecision;
        [SerializeField] private Transform instantiationParent;
        [SerializeField] private LootableView lootableViewPrefab;
        [SerializeField] private Button passButton;
        [SerializeField] private int lootCount;

        private List<LootableView> _instantiatedLootables;

        private void Awake()
        {
            LootingState.onInstantiateLootable += InstantiateLootables;
            
            passButton.onClick.AddListener(() =>
            {
                lootIndexRoomDecision.SetDecision(-1);
                passButton.interactable = false;
            });
        }

        private void OnDestroy()
        {
            LootingState.onInstantiateLootable -= InstantiateLootables;
            
            passButton.onClick.RemoveAllListeners();
        }
        
        private List<LootableView> InstantiateLootables()
        {
            _instantiatedLootables = new List<LootableView>();
            
            for (int i = 0; i < battleData.lootables.Count; i++)
            {
                LootableView lootableView = Instantiate(lootableViewPrefab, instantiationParent);
                _instantiatedLootables.Add(lootableView);
                lootableView.Initialize(battleData.lootables[i], battleData.lootableStages[i], () =>
                {
                    int findIndex = _instantiatedLootables.FindIndex(x => x == lootableView);
                    lootIndexRoomDecision.SetDecision(findIndex);
                });
                
                if (i > lootCount)
                {
                    _instantiatedLootables[i].gameObject.SetActive(false);
                }
            }
            
            return _instantiatedLootables;
        }
        
        private void EnableNew()
        {
            int count = Mathf.Min(lootCount, _instantiatedLootables.Count);

            for (int i = 0; i < count; i++)
            {
                _instantiatedLootables[i].gameObject.SetActive(true);
            }
        }
        
        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            lootIndexRoomDecision.IsValidDecision(() =>
            {
                passButton.interactable = true;
                DestroyOtherChoices();
                TryTakeSelectedLootable();

                if (!IsLootableRemaining())
                {
                    EnableNew();
                }
            });
        }
        
        private void DestroyOtherChoices()
        {
            Hashtable roomCustomProperties = PhotonNetwork.CurrentRoom.CustomProperties;

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (Equals(player, PhotonNetwork.LocalPlayer)) continue;

                int index = (int) roomCustomProperties[lootIndexRoomDecision.Identifier(player)];
                RemoveFromLootSlots(index);
            }
        }
        
        private void TryTakeSelectedLootable()
        {
            Hashtable roomCustomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
            Player localPlayer = PhotonNetwork.LocalPlayer;
        
            if (!roomCustomProperties.ContainsKey(lootIndexRoomDecision.Identifier(localPlayer))) return;
            
            int ownDecisionIndex = (int)roomCustomProperties[lootIndexRoomDecision.Identifier(localPlayer)];
            if (ownDecisionIndex >= 0 && _instantiatedLootables[ownDecisionIndex] != null)
            {
                _instantiatedLootables[ownDecisionIndex].GenerateContainedLootable();
                RemoveFromLootSlots(ownDecisionIndex);
            }
        }
        
        private void RemoveFromLootSlots(int index)
        {
            if (index < 0)
            {
                return;
            }
            
            LootableView lootableViewToBeRemoved = _instantiatedLootables[index];
            Destroy(lootableViewToBeRemoved.gameObject);
            _instantiatedLootables.RemoveAt(index);
        }
        
        private bool IsLootableRemaining()
        {
            return _instantiatedLootables.Any(t => t != null);
        }
    }
}
