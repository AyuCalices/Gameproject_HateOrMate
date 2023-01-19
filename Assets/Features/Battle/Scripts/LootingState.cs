using System;
using System.Collections;
using System.Collections.Generic;
using Features.Battle.StateMachine;
using Features.Loot.Scripts.LootView;
using UniRx;
using UnityEngine;

namespace Features.Battle.Scripts
{
    [CreateAssetMenu]
    public class LootingState : BaseCoroutineState
    {
        public static Func<List<LootableView>> onInstantiateLootable;
        
        [SerializeField] private BattleData_SO battleData;
        
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
            
            Debug.Log("Enter Looting State - Before");
            yield return new WaitForSeconds(2f);
            Debug.Log("Enter Looting State - After");

            if (battleData.lootables.Count == 0)
            {
                _battleManager.RequestPlacementState();
            }
            else
            {
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
            
            Debug.Log("Exit Looting State - Before");
            yield return new WaitForSeconds(2f);
            Debug.Log("Exit Looting State - After");
        }
    }
}
