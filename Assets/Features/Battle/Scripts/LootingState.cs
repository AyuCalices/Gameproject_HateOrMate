using System;
using System.Collections;
using System.Collections.Generic;
using Features.Battle.StateMachine;
using Features.Connection.Scripts;
using Features.Loot.Scripts.LootView;
using Features.Loot.Scripts.ModView;
using UniRx;
using UnityEngine;

namespace Features.Battle.Scripts
{
    [CreateAssetMenu]
    public class LootingState : BaseCoroutineState
    {
        public static Func<List<LootableView>> onInstantiateLootable;
        
        [SerializeField] private BattleData_SO battleData;
        [SerializeField] private NotePopup notePopupPrefab;
        [SerializeField] private CanvasFocus_SO canvasFocus;
        
        private BattleManager _battleManager;
        private bool _initialized;
        private IDisposable _disposable;
        
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
            
            if (battleData.lootables.Count == 0)
            {
                yield return notePopupPrefab.Instantiate(canvasFocus.Get().transform, "There are no Lootables!");
                
                _battleManager.RequestPlacementState();
            }
            else
            {
                yield return notePopupPrefab.Instantiate(canvasFocus.Get().transform, "Take Your Items!");
                
                List<LootableView> lootables = onInstantiateLootable.Invoke();
                _disposable = lootables.ObserveEveryValueChanged(list => list.Count)
                    .Where(count => count == 0)
                    .Subscribe(_ =>
                    {
                        battleData.lootables.Clear();
                        battleData.lootableStages.Clear();
                        _battleManager.RequestPlacementState();
                    });
            }
        }

        public override IEnumerator Exit()
        {
            yield return base.Exit();
            
            _disposable?.Dispose();
        }
    }
}
