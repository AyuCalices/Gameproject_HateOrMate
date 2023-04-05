using System.Collections;
using Features.BattleScene.Scripts.StateMachine;
using Features.General.Photon.Scripts;
using Features.General.UI.Scripts;
using Features.General.UI.Scripts.CanvasFocus;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Features.BattleScene.Scripts.States
{
    [CreateAssetMenu]
    public class PlacementState : BaseCoroutineState
    {
        [Header("Derived References")]
        [SerializeField] private BoolRoomDecitions_SO continueButtonRoomDecision;
        [SerializeField] private NotePopup notePopupPrefab;
        [SerializeField] private CanvasFocus_SO canvasFocus;

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
        
            yield return notePopupPrefab.Instantiate(canvasFocus.Get().transform, "Place Your Units on the Grid!");
        }
        
        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            continueButtonRoomDecision.IsValidDecision(() => _battleManager.RequestStageSetupState(), b => b);
        }
    }
}
