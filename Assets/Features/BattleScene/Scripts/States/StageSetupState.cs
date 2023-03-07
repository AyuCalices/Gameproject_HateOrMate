using System.Collections;
using ExitGames.Client.Photon;
using Features.BattleScene.Scripts.StageProgression;
using Features.BattleScene.Scripts.StateMachine;
using Features.General.Photon.Scripts;
using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Services.UnitStats;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Features.BattleScene.Scripts.States
{
    [CreateAssetMenu]
    public class StageSetupState : BaseCoroutineState
    {
        [Header("Derived References")]
        [SerializeField] private BattleData_SO battleData;
        [SerializeField] private StageRandomizer_SO stageRandomizer;
        
        private BattleManager _battleManager;
        private bool _initialized;
        
        private void OnEnable()
        {
            _initialized = false;
        }

        public StageSetupState Initialize(BattleManager battleManager)
        {
            if (_initialized) return this;
            
            _battleManager = battleManager;
            
            return this;
        }

        public override IEnumerator Enter()
        {
            yield return base.Enter();

            if (!battleData.IsStageRestart)
            {
                //TODO: unit leveling is not synchronized
                battleData.Stage.Add(1);

                foreach (UnitServiceProvider unitServiceProvider in battleData.UnitsServiceProviderRuntimeSet.GetItems())
                {
                    if (unitServiceProvider.UnitClassData.levelUpOnStageClear)
                    {
                        unitServiceProvider.GetService<UnitStatsBehaviour>().SetBaseStats(unitServiceProvider.UnitClassData.baseStatsData, battleData.Stage.Get());
                    }
                }
            }

            if (PhotonNetwork.IsMasterClient)
            {
                stageRandomizer.GenerateUnits();
                RequestBattleStateByRaiseEvent();
            }
        }

        public override void OnEvent(EventData photonEvent)
        {
            if (!_battleManager.StateIsValid(typeof(StageSetupState), StateProgressType.Execute)) return;
            
            if (photonEvent.Code == (int)RaiseEventCode.OnRequestBattleState)
            {
                _battleManager.RequestBattleState();
            }
        }

        private void RequestBattleStateByRaiseEvent()
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
            
            PhotonNetwork.RaiseEvent((int)RaiseEventCode.OnRequestBattleState, null, raiseEventOptions, sendOptions);
        }
    }
}
