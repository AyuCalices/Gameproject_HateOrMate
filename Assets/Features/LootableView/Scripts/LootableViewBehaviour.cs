using System;
using System.Collections.Generic;
using System.Linq;
using Features.Battle.Scripts;
using Features.Connection.Scripts.Utils;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Features.Loot.Scripts.LootView
{
    public class LootableViewBehaviour : MonoBehaviourPunCallbacks
    {
        [SerializeField] private BattleData_SO battleData;
        [SerializeField] private IntRoomDecitions_SO lootIndexRoomDecision;
        [SerializeField] private Transform instantiationParent;
        [FormerlySerializedAs("lootableViewPrefab")] [SerializeField] private LootableDisplayBehaviour lootableDisplayBehaviourPrefab;
        [SerializeField] private Button passButton;
        [SerializeField] private int lootCount;

        private List<LootableDisplayBehaviour> _instantiatedLootables;

        private void Awake()
        {
            LootingState.onInstantiateLootable += InstantiateLootables;
            
            passButton.onClick.AddListener(() =>
            {
                lootIndexRoomDecision.SetDecision(-1);
            });
        }

        private void OnDestroy()
        {
            LootingState.onInstantiateLootable -= InstantiateLootables;
            
            passButton.onClick.RemoveAllListeners();
        }
        
        private List<LootableDisplayBehaviour> InstantiateLootables()
        {
            _instantiatedLootables = new List<LootableDisplayBehaviour>();
            
            for (int i = 0; i < battleData.Lootables.Count; i++)
            {
                LootableDisplayBehaviour lootableDisplayBehaviour = Instantiate(lootableDisplayBehaviourPrefab, instantiationParent);
                _instantiatedLootables.Add(lootableDisplayBehaviour);
                lootableDisplayBehaviour.Initialize(battleData.Lootables[i], battleData.LootableStages[i], i,lootIndexRoomDecision.SetDecision);
                
                if (i >= lootCount)
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
                passButton.GetComponent<SingleSelectionBehaviour>().DisableCurrentSelection();
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

            foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
            {
                if (Equals(player.Value, PhotonNetwork.LocalPlayer)) continue;

                int lootableIdentifier = (int) roomCustomProperties[lootIndexRoomDecision.Identifier(player.Value)];
                RemoveFromLootSlots(GetLootableIndex(lootableIdentifier));
            }
        }

        private int GetLootableIndex(int identifier)
        {
            return _instantiatedLootables.FindIndex(x => x.Identifier == identifier);
        }
        
        private void TryTakeSelectedLootable()
        {
            Hashtable roomCustomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
            Player localPlayer = PhotonNetwork.LocalPlayer;
        
            if (!roomCustomProperties.ContainsKey(lootIndexRoomDecision.Identifier(localPlayer))) return;
            
            int lootableIdentifier = (int)roomCustomProperties[lootIndexRoomDecision.Identifier(localPlayer)];
            if (lootableIdentifier < 0) return;
            int ownDecisionIndex = GetLootableIndex(lootableIdentifier);
            if (ownDecisionIndex < 0) return;
            _instantiatedLootables[ownDecisionIndex].GenerateContainedLootable();
            RemoveFromLootSlots(ownDecisionIndex);
        }
        
        private void RemoveFromLootSlots(int index)
        {
            if (index < 0)
            {
                return;
            }
            
            LootableDisplayBehaviour lootableDisplayBehaviourToBeRemoved = _instantiatedLootables[index];
            Destroy(lootableDisplayBehaviourToBeRemoved.gameObject);
            _instantiatedLootables.RemoveAt(index);
        }
        
        private bool IsLootableRemaining()
        {
            return _instantiatedLootables.Any(t => t.gameObject.activeSelf);
        }
    }
}
