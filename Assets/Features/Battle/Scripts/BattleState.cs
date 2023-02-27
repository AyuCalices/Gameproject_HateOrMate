using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Features.Battle.StateMachine;
using Features.Connection.Scripts;
using Features.Connection.Scripts.Utils;
using Features.Loot.Scripts.Generator;
using Features.Loot.Scripts.ModView;
using Features.Unit.Scripts;
using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Battle;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Features.Battle.Scripts
{
    [CreateAssetMenu]
    public class BattleState : BaseCoroutineState
    {
        [Header("Derived References")]
        [SerializeField] private BoolRoomDecitions_SO requestLootPhaseButtonRoomDecision;
        [SerializeField] private int lootCountOnStageComplete;
        [SerializeField] private BattleData_SO battleData;
        [SerializeField] private NotePopup notePopupPrefab;
        [SerializeField] private CanvasFocus_SO canvasFocus;
        
        private BattleManager _battleManager;
        private bool _initialized;

        private void OnEnable()
        {
            _initialized = false;
        }

        public BattleState Initialize(BattleManager battleManager)
        {
            if (_initialized) return this;
            
            _battleManager = battleManager;

            return this;
        }

        public override IEnumerator Enter()
        {
            yield return base.Enter();
            
            DeathState.onUnitEnterDeathState += SetStage;

            if (!battleData.IsStageRestart)
            {
                yield return notePopupPrefab.Instantiate(canvasFocus.Get().transform, "Next Stage!");
            }
            else
            {
                yield return notePopupPrefab.Instantiate(canvasFocus.Get().transform, "Stage Starts!");
            }
        }

        public override IEnumerator Exit()
        {
            yield return base.Exit();
            
            DeathState.onUnitEnterDeathState -= SetStage;

            if (battleData.IsStageRestart)
            {
                yield return notePopupPrefab.Instantiate(canvasFocus.Get().transform, "Stage Failed!");
            }

            var aiUnits = battleData.AllUnitsRuntimeSet.GetUnitsByTag(TeamTagType.AI);
            for (int index = aiUnits.Count - 1; index >= 0; index--)
            {
                NetworkedBattleBehaviour networkedBattleBehaviour = aiUnits[index];
                networkedBattleBehaviour.Destroy();
            }
            
            foreach (NetworkedBattleBehaviour networkedUnitBehaviour in battleData.AllUnitsRuntimeSet.GetItems())
            {
                networkedUnitBehaviour.OnStageEnd();
                networkedUnitBehaviour.NetworkedStatsBehaviour.RemovedHealth = 0;
            }
        }

        private void SetStage()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            
            if (battleData.Stage.Get() >= battleData.CompletedStage.Get())
            {
                EndGame_RaiseEvent();
                return;
            }

            if (!HasUnitAlive(TeamTagType.Mate, TeamTagType.Own))
            {
                bool enterLootingState = requestLootPhaseButtonRoomDecision.IsValidDecision(null, x => x);
                RestartStage_RaiseEvent(enterLootingState);
                return;
            }

            if (!HasUnitAlive(TeamTagType.AI))
            {
                bool enterLootingState = requestLootPhaseButtonRoomDecision.IsValidDecision(null, x => x);
                LootableGenerator_SO[] lootables = RandomizeLootables();
                NextStage_RaiseEvent(enterLootingState, lootables, battleData.Stage.Get());
            }
        }
        
        private bool HasUnitAlive(params TeamTagType[] tagTypes)
        {
            return battleData.AllUnitsRuntimeSet.GetUnitsByTag(tagTypes)
                .Any(e => e.CurrentState is not DeathState && e.IsTargetable && e.CurrentState is not BenchedState);
        }
        
        //out
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
        
        private void EndGame_RaiseEvent()
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.All,
                CachingOption = EventCaching.AddToRoomCache
            };

            SendOptions sendOptions = new SendOptions
            {
                Reliability = true
            };
            
            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnEndGame, null, raiseEventOptions, sendOptions);
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
            if (!_battleManager.StateIsValid(typeof(BattleState), StateProgressType.Execute)) return;

            switch (photonEvent.Code)
            {
                case (int)RaiseEventCode.OnAttack:
                {
                    object[] data = (object[]) photonEvent.CustomData;
                    
                    if (battleData.AllUnitsRuntimeSet.GetUnitByViewID((int) data[0], out NetworkedBattleBehaviour networkedUnitBehaviour))
                    {
                        networkedUnitBehaviour.BattleClass.AttackCallback((float) data[1], (float) data[2], (UnitClassData_SO) data[3]);
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
                    LootableGenerator_SO[] lootables = (LootableGenerator_SO[]) data[1];
                    int stageAsLevel = (int) data[2];

                    foreach (var lootable in lootables)
                    {
                        battleData.Lootables.Add(lootable);
                        battleData.LootableStages.Add(stageAsLevel);
                    }
                    
                    bool enterLootingState = (bool) data[0];
                    battleData.IsStageRestart = false;
                    EndStage(enterLootingState);
                    break;
                }
                case (int)RaiseEventCode.OnEndGame:
                    _battleManager.RequestEndGameState();
                    break;
            }
        }

        private void EndStage(bool enterLootingState)
        {
            if (enterLootingState)
            {
                _battleManager.RequestLootingState();
            }
            else
            {
                _battleManager.RequestStageSetupState();
            }
        }
    }
}
