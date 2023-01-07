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
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Features.Loot.Scripts.LootView
{
    public class LootSelectionBehaviour : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        [SerializeField] private Transform instantiationParent;
        [SerializeField] private LootableView lootableViewPrefab;
        [SerializeField] private int lootCount;

        private LootableView[] _instantiatedLootables;
        private RoomDecisions<int> _roomDecisions;
        private readonly List<LootableGenerator_SO> _lootables = new List<LootableGenerator_SO>();
        private readonly List<int> _lootableStages = new List<int>();

        private void Awake()
        {
            _instantiatedLootables = new LootableView[lootCount];
            _roomDecisions = new RoomDecisions<int>("Looting", false);
            gameObject.SetActive(false);
            
            PhotonNetwork.AddCallbackTarget(this);
        }

        private void OnDestroy()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        private void Start()
        {
            this.UpdateAsObservable()
                .Where(_ => !IsLootableRemaining())
                .Subscribe(_ => ShowNewLootablesOrClose());
            
            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    _roomDecisions.UpdateDecision(() =>
                    {
                        DestroyOtherChoices();
                        TryTakeSelectedLootable();
                    });
                });
        }

        public override void OnEnable()
        {
            ShowNewLootablesOrClose();
        }
        
        public override void OnDisable()
        {
            foreach (LootableView lootable in _instantiatedLootables)
            {
                Destroy(lootable);
            }
        }

        private void ShowNewLootablesOrClose()
        {
            if (_lootables.Count == 0)
            {
                gameObject.SetActive(false);
                return;
            }
            
            int range = Mathf.Min(lootCount, _lootables.Count);
            
            for (int i = 0; i < range; i++)
            {
                LootableView lootableView = Instantiate(lootableViewPrefab, instantiationParent);
                int localScope = i;
                lootableView.Initialize(_lootables[i], _lootableStages[i], () => _roomDecisions.SetLocalDecision(localScope));
                _instantiatedLootables[i] = lootableView;
            }
            
            _lootables.RemoveRange(0, range);
            _lootableStages.RemoveRange(0, range);
        }

        private void TryTakeSelectedLootable()
        {
            Hashtable roomCustomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
            Player localPlayer = PhotonNetwork.LocalPlayer;
        
            if (!roomCustomProperties.ContainsKey(_roomDecisions.Identifier(localPlayer))) return;
            
            int ownDecisionIndex = (int)roomCustomProperties[_roomDecisions.Identifier(localPlayer)];
            if (_instantiatedLootables[ownDecisionIndex] != null)
            {
                _instantiatedLootables[ownDecisionIndex].GenerateContainedLootable();
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

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == (int)RaiseEventCode.OnNextStage)
            {
                object[] data = (object[]) photonEvent.CustomData;
                bool enterLootingState = (bool) data[0];
                LootableGenerator_SO[] lootables = (LootableGenerator_SO[]) data[1];
                int stageAsLevel = (int) data[2];

                foreach (var lootable in lootables)
                {
                    _lootables.Add(lootable);
                    _lootableStages.Add(stageAsLevel);
                }

                if (enterLootingState)
                {
                    gameObject.SetActive(true);
                }
            }
        }
    }
}
