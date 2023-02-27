using System.Collections;
using System.Linq;
using ExitGames.Client.Photon;
using Features.Battle.Scripts.StageProgression;
using Features.Battle.Scripts.Unit.ServiceLocatorSystem;
using Features.Battle.StateMachine;
using Features.Connection.Scripts.Utils;
using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Battle;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Features.Battle.Scripts
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
                battleData.Stage.Add(1);

                foreach (UnitServiceProvider unitServiceProvider in battleData.AllUnitsRuntimeSet.GetItems())
                {
                    if (unitServiceProvider.GetService<NetworkedBattleBehaviour>().TeamTagTypes.Contains(TeamTagType.Own) && unitServiceProvider.GetService<NetworkedBattleBehaviour>().UnitClassData.levelUpOnStageClear)
                    {
                        unitServiceProvider.GetService<NetworkedStatsBehaviour>().SetBaseStats(unitServiceProvider.GetService<NetworkedBattleBehaviour>().UnitClassData.baseStatsData, battleData.Stage.Get());
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
