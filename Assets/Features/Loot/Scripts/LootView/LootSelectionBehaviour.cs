using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Features.Connection.Scripts.Utils;
using Features.Loot.Scripts.Generator;
using Photon.Pun;
using Photon.Realtime;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Features.Loot.Scripts.LootView
{
    public class LootSelectionBehaviour : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        [SerializeField] private IntRoomDecitions_SO lootIndexRoomDecision;
        [SerializeField] private Transform instantiationParent;
        [SerializeField] private LootableView lootableViewPrefab;
        [SerializeField] private Button passButton;
        [SerializeField] private int lootCount;

        private LootableView[] _instantiatedLootables;
        private readonly List<LootableGenerator_SO> _lootables = new List<LootableGenerator_SO>();
        private readonly List<int> _lootableStages = new List<int>();

        private void Awake()
        {
            _instantiatedLootables = new LootableView[lootCount];

            PhotonNetwork.AddCallbackTarget(this);
        }

        private void OnDestroy()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        public override void OnEnable()
        {
            passButton.onClick.AddListener(() =>
            {
                lootIndexRoomDecision.SetDecision(-1);
                passButton.interactable = false;
            });
        }
        
        public override void OnDisable()
        {
            foreach (LootableView lootable in _instantiatedLootables)
            {
                Destroy(lootable);
            }
            
            passButton.onClick.RemoveAllListeners();
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
                    ShowNewLootablesOrClose();
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
            if (index < 0) return;
            
            LootableView lootableViewToBeRemoved = _instantiatedLootables[index];
            Destroy(lootableViewToBeRemoved.gameObject);
            _instantiatedLootables[index] = null;
        }
        
        private bool IsLootableRemaining()
        {
            return _instantiatedLootables.Any(t => t != null);
        }

        private void ShowNewLootablesOrClose()
        {
            if (_lootables.Count == 0) return;

            int range = Mathf.Min(lootCount, _lootables.Count);
            
            for (int i = 0; i < range; i++)
            {
                LootableView lootableView = Instantiate(lootableViewPrefab, instantiationParent);
                int lootableIndex = i;
                lootableView.Initialize(_lootables[lootableIndex], _lootableStages[lootableIndex], () => lootIndexRoomDecision.SetDecision(lootableIndex));
                _instantiatedLootables[i] = lootableView;
            }
            
            _lootables.RemoveRange(0, range);
            _lootableStages.RemoveRange(0, range);
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == (int)RaiseEventCode.OnNextStage)
            {
                object[] data = (object[]) photonEvent.CustomData;
                LootableGenerator_SO[] lootables = (LootableGenerator_SO[]) data[1];
                int stageAsLevel = (int) data[2];

                foreach (var lootable in lootables)
                {
                    _lootables.Add(lootable);
                    _lootableStages.Add(stageAsLevel);
                }

                ShowNewLootablesOrClose();
            }
        }
    }
}
