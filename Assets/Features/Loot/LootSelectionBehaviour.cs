using System.Linq;
using Features.Battle;
using Features.GlobalReferences;
using Features.ModView;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Features.Loot
{
    public class LootSelectionBehaviour : MonoBehaviour
    {
        [SerializeField] private BattleData_SO battleData;
        [SerializeField] private NetworkedUnitRuntimeSet_SO localUnitRuntimeSet;
        [SerializeField] private ModDragBehaviour modDragBehaviourPrefab;
        [SerializeField] private Transform instantiationParent;
        [SerializeField] private LootableView lootableViewPrefab;
        [SerializeField] private int lootCount;

        private LootableView[] _instantiatedLootables;
        private Hashtable _localDecision;

        private void Awake()
        {
            _instantiatedLootables = new LootableView[lootCount];
            
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
                lootableView.Initialize(battleData.lootables[i], () => SetLocalDecision(localScope));
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
                    Debug.Log("all mods chosen");
                    gameObject.SetActive(false);
                }
                else
                {
                    ShowNew();
                }
            }
            
            if (!AllPlayerChose()) return;
            DestroyOtherChoices();
            TryTakeSelectedLootable();
            ResetLocalDecision();
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
        
            if (!roomCustomProperties.ContainsKey(localPlayer.ActorNumber.ToString())) return;
            
            int ownDecision = (int)roomCustomProperties[localPlayer.ActorNumber.ToString()];
            if (_instantiatedLootables[ownDecision] != null)
            {
                localUnitRuntimeSet.TryInstantiateModToAny(modDragBehaviourPrefab, _instantiatedLootables[ownDecision].LootableGenerator.Generate());
                RemoveFromLootSlots(ownDecision);
            }
        }

        private void DestroyOtherChoices()
        {
            Hashtable roomCustomProperties = PhotonNetwork.CurrentRoom.CustomProperties;

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (Equals(player, PhotonNetwork.LocalPlayer)) continue;

                int index = (int) roomCustomProperties[player.ActorNumber.ToString()];
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
        
        private bool AllPlayerChose()
        {
            Hashtable roomCustomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (Equals(player, PhotonNetwork.LocalPlayer) && _localDecision != null && _localDecision[player.ActorNumber.ToString()] == null)
                {
                    return false;
                }
                
                if (!roomCustomProperties.ContainsKey(player.ActorNumber.ToString()) || roomCustomProperties[player.ActorNumber.ToString()] == null)
                {
                    return false;
                }
            }

            return true;
        }

        private void SetLocalDecision(int index)
        {
            Player localPlayer = PhotonNetwork.LocalPlayer;
            _localDecision = new Hashtable(){{localPlayer.ActorNumber.ToString(), index}};
            PhotonNetwork.CurrentRoom.SetCustomProperties(_localDecision);
        }

        private void ResetLocalDecision()
        {
            Player localPlayer = PhotonNetwork.LocalPlayer;
            _localDecision = new Hashtable(){{localPlayer.ActorNumber.ToString(), null}};
            PhotonNetwork.CurrentRoom.SetCustomProperties(_localDecision);
        }
    }
}
