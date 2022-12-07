using System.Linq;
using Features.Battle;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Features.Loot
{
    public class LootSelectionBehaviour : MonoBehaviour
    {
        [SerializeField] private BattleData_SO battleData;
        [SerializeField] private Transform instantiationParent;
        [SerializeField] private LootableView lootableViewPrefab;
        [SerializeField] private int lootCount;

        private LootableView[] _instantiatedLootables;
        private RoomDecisions<int> _roomDecisions;

        private void Awake()
        {
            _instantiatedLootables = new LootableView[lootCount];
            _roomDecisions = new RoomDecisions<int>("Looting");
            
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            ShowNew();
        }

        private void ShowNew()
        {
            int range = Mathf.Min(lootCount, battleData.lootables.Count);
            
            for (int i = 0; i < range; i++)
            {
                LootableView lootableView = Instantiate(lootableViewPrefab, instantiationParent);
                int localScope = i;
                lootableView.Initialize(battleData.lootables[i], () => _roomDecisions.SetLocalDecision(localScope));
                _instantiatedLootables[i] = lootableView;
            }
            
            battleData.lootables.RemoveRange(0, range);
        }

        private void Update()
        {
            if (!IsLootableRemaining())
            {
                if (battleData.lootables.Count == 0)
                {
                    gameObject.SetActive(false);
                }
                else
                {
                    ShowNew();
                }
            }
            
            _roomDecisions.UpdateDecision(() =>
            {
                DestroyOtherChoices();
                TryTakeSelectedLootable();
            });
        }

        private void OnDisable()
        {
            foreach (LootableView lootable in _instantiatedLootables)
            {
                Destroy(lootable);
            }
        }

        private void TryTakeSelectedLootable()
        {
            Hashtable roomCustomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
            Player localPlayer = PhotonNetwork.LocalPlayer;
        
            if (!roomCustomProperties.ContainsKey(_roomDecisions.Identifier(localPlayer))) return;
            
            int ownDecisionIndex = (int)roomCustomProperties[_roomDecisions.Identifier(localPlayer)];
            if (_instantiatedLootables[ownDecisionIndex] != null)
            {
                _instantiatedLootables[ownDecisionIndex].LootableGenerator.OnAddInstanceToPlayer();
                RemoveFromLootSlots(ownDecisionIndex);
            }
        }

        private void DestroyOtherChoices()
        {
            Hashtable roomCustomProperties = PhotonNetwork.CurrentRoom.CustomProperties;

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (Equals(player, PhotonNetwork.LocalPlayer)) continue;

                int index = (int) roomCustomProperties[_roomDecisions.Identifier(player)];
                RemoveFromLootSlots(index);
            }
        }

        private void RemoveFromLootSlots(int index)
        {
            LootableView lootableViewToBeRemoved = _instantiatedLootables[index];
            Destroy(lootableViewToBeRemoved.gameObject);
            _instantiatedLootables[index] = null;
        }

        private bool IsLootableRemaining()
        {
            return _instantiatedLootables.Any(t => t != null);
        }
    }
}
