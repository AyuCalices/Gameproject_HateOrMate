using System;
using System.Collections;
using System.Collections.Generic;
using Features.BattleScene.Scripts.StateMachine;
using Features.General.UI.Scripts;
using Features.General.UI.Scripts.CanvasFocus;
using Features.Loot.Scripts;
using UnityEngine;

namespace Features.BattleScene.Scripts.States
{
    [CreateAssetMenu]
    public class LootingState : BaseCoroutineState
    {
        public static Func<List<LootableDisplayBehaviour>> onInstantiateLootable;
        
        [SerializeField] private BattleData_SO battleData;
        [SerializeField] private NotePopup notePopupPrefab;
        [SerializeField] private CanvasFocus_SO canvasFocus;
        
        private BattleManager _battleManager;
        private bool _initialized;
        private List<LootableDisplayBehaviour> _lootables;
        
        private void OnEnable()
        {
            _initialized = false;
        }

        public LootingState Initialize(BattleManager battleManager)
        {
            if (_initialized) return this;
            
            _battleManager = battleManager;

            return this;
        }

        public override IEnumerator Enter()
        {
            yield return base.Enter();
            
            if (battleData.Lootables.Count == 0)
            {
                yield return notePopupPrefab.Instantiate(canvasFocus.Get().transform, "There are no Lootables!");
                
                _battleManager.RequestPlacementState();
            }
            else
            {
                yield return notePopupPrefab.Instantiate(canvasFocus.Get().transform, "Take Your Items!");
                
                _lootables = onInstantiateLootable.Invoke();
            }
        }

        public override IEnumerator Execute()
        {
            yield return base.Execute();

            if (_lootables.Count != 0 && _battleManager.StateIsValid(typeof(LootingState), StateProgressType.Execute)) yield break;
            
            battleData.Lootables.Clear();
            battleData.LootableStages.Clear();
            _battleManager.RequestPlacementState();
        }

        public override IEnumerator Exit()
        {
            yield return base.Exit();
        }
    }
}