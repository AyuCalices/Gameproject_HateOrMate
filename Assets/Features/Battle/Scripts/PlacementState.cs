using System.Collections;
using Features.Battle.StateMachine;
using Features.Connection.Scripts.Utils;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Features.Battle.Scripts
{
    [CreateAssetMenu]
    public class PlacementState : BaseCoroutineState
    {
        [Header("Derived References")]
        [SerializeField] private BoolRoomDecitions_SO continueButtonRoomDecision;

        private BattleManager _battleManager;
        private bool _initialized;
        
        private void OnEnable()
        {
            _initialized = false;
        }

        public PlacementState Initialize(BattleManager battleManager)
        {
            if (_initialized) return this;
            
            _battleManager = battleManager;

            return this;
        }
        
        public override IEnumerator Enter()
        {
            yield return base.Enter();
        
            Debug.Log("Enter Placement State");
        }

        public override IEnumerator Exit()
        {
            yield return base.Exit();
            
            Debug.Log("Exit Placement State");
        }
        
        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            continueButtonRoomDecision.IsValidDecision(() => _battleManager.RequestStageSetupState(), b => b);
        }
    }
}
