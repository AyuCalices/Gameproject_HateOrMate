using System;
using System.Collections;
using ExitGames.Client.Photon;
using Features.Battle.StateMachine;
using Features.Connection.Scripts.Utils;
using Features.Loot.Scripts.Generator;
using Features.Unit.Scripts;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Behaviours.Stat;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Battle.Scripts
{
    [CreateAssetMenu]
    public class BattleState : BaseBattleState_SO
    {
        public static Action onLocalDespawnAllUnits;
        
        [SerializeField] private int lootCountOnStageComplete;
        public BattleData_SO battleData;
        
        private BattleManager _battleManager;
        private Button _requestLootPhaseButton;
        
        private RoomDecisions<bool> _enterLootingPhaseRoomDecision;
        
        private bool _initialized;

        public BattleState Initialize(BattleManager battleManager, Button requestLootPhaseButton)
        {
            if (_initialized) return this;
            
            _battleManager = battleManager;
            _requestLootPhaseButton = requestLootPhaseButton;
            
            _enterLootingPhaseRoomDecision = new RoomDecisions<bool>("EnterLootingPhase", true);
            requestLootPhaseButton.onClick.AddListener(() =>
            {
                _enterLootingPhaseRoomDecision.SetDecision(true);
                requestLootPhaseButton.interactable = false;
            });

            return this;
        }

        public override IEnumerator Enter()
        {
            yield return base.Enter();
            
            _requestLootPhaseButton.interactable = true;
            NetworkedStatsBehaviour.onDamageGained += CheckStage;

            Debug.Log("Enter Battle State - Before");
            yield return new WaitForSeconds(2f);
            Debug.Log("Enter Battle State - After");
        }

        public override IEnumerator Exit()
        {
            yield return base.Exit();
            
            NetworkedStatsBehaviour.onDamageGained -= CheckStage;
            
            Debug.Log("Exit Battle State - Before");
            yield return new WaitForSeconds(2f);
            Debug.Log("Exit Battle State - After");
        }

        private void CheckStage(NetworkedBattleBehaviour networkedBattleBehaviour, float newRemovedHealth, float totalHealth)
        {
            if (newRemovedHealth >= totalHealth)
            {
                networkedBattleBehaviour.TryRequestDeathState();
                SetStage();
            }
        }

        private void SetStage()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            
            bool enterLootingState = _enterLootingPhaseRoomDecision.IsValidDecision(null, x => x);
            
            if (!battleData.PlayerUnitsRuntimeSet.HasUnitAlive())
            {
                RestartStage_RaiseEvent(enterLootingState);
                return;
            }

            if (!battleData.EnemyUnitsRuntimeSet.HasUnitAlive())
            {
                LootableGenerator_SO[] lootables = RandomizeLootables();
                NextStage_RaiseEvent(enterLootingState, lootables, battleData.Stage.Get());
            }
        }
        
        private LootableGenerator_SO[] RandomizeLootables()
        {
            LootableGenerator_SO[] lootables = new LootableGenerator_SO[lootCountOnStageComplete];
            for (int index = 0; index < lootCountOnStageComplete; index++)
            {
                lootables[index] = battleData.LootTable.RandomizeLootableGenerator();
            }

            return lootables;
        }

        private void NextStage_RaiseEvent(bool enterLootingState, LootableGenerator_SO[] lootable, int currentStageAsLevel)
        {
            object[] data = new object[]
            {
                enterLootingState,
                lootable,
                currentStageAsLevel
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
            
            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnNextStage, data, raiseEventOptions, sendOptions);
        }
        
        private void RestartStage_RaiseEvent(bool enterLootingState)
        {
            object[] data = new object[]
            {
                enterLootingState
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
            
            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnRestartStage, data, raiseEventOptions, sendOptions);
        }

        public override void OnEvent(EventData photonEvent)
        {
            switch (photonEvent.Code)
            {
                case (int)RaiseEventCode.OnSendFloatToTarget:
                {
                    object[] data = (object[]) photonEvent.CustomData;
                    if (battleData.AllUnitsRuntimeSet.TryGetUnitByViewID((int) data[0], out NetworkedBattleBehaviour networkedUnitBehaviour)
                        && networkedUnitBehaviour is BattleBehaviour battleBehaviour)
                    {
                        battleBehaviour.BattleClass.OnReceiveFloatActionCallback((float) data[1], (UnitType_SO) data[2]);
                    }

                    break;
                }
                case (int)RaiseEventCode.OnUpdateAllClientsHealth:
                {
                    object[] data = (object[]) photonEvent.CustomData;
                    if (battleData.AllUnitsRuntimeSet.TryGetUnitByViewID((int) data[0], out NetworkedBattleBehaviour networkedUnitBehaviour))
                    {
                        OnUpdateAllClientsHealthCallback(networkedUnitBehaviour, (float) data[1], (float) data[2]);
                    }

                    break;
                }
                case (int)RaiseEventCode.OnRestartStage:
                {
                    object[] data = (object[]) photonEvent.CustomData;
                    bool enterLootingState = (bool) data[0];
                    battleData.IsStageRestart = true;
                    EndStage(enterLootingState);
                    break;
                }
                case (int)RaiseEventCode.OnNextStage:
                {
                    object[] data = (object[]) photonEvent.CustomData;
                    bool enterLootingState = (bool) data[0];
                    battleData.IsStageRestart = false;
                    EndStage(enterLootingState);
                    break;
                }
            }
        }

        private void EndStage(bool enterLootingState)
        {
            onLocalDespawnAllUnits?.Invoke();
            
            foreach (NetworkedBattleBehaviour networkedUnitBehaviour in battleData.AllUnitsRuntimeSet.GetItems())
            {
                networkedUnitBehaviour.OnStageEnd();
                networkedUnitBehaviour.NetworkedStatsBehaviour.RemovedHealth = 0;
            }
            
            if (enterLootingState)
            {
                _battleManager.RequestLootingState();
            }
            else
            {
                _battleManager.RequestStageSetupState();
            }
        }
        
        /// <summary>
        /// All players update this units health
        /// </summary>
        /// <param name="newRemovedHealth"></param>
        /// <param name="totalHealth"></param>
        private void OnUpdateAllClientsHealthCallback(NetworkedBattleBehaviour networkedBattleBehaviour, float newRemovedHealth,
            float totalHealth)
        {
            NetworkedStatsBehaviour networkedStatsBehaviour = networkedBattleBehaviour.NetworkedStatsBehaviour;
            networkedStatsBehaviour.RemovedHealth = newRemovedHealth;
        
            CheckStage(networkedBattleBehaviour, newRemovedHealth, totalHealth);
        }
    }
}
