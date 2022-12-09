using System.Collections.Generic;
using DataStructures.ReactiveVariable;
using DataStructures.StateLogic;
using Features.GlobalReferences;
using Features.GlobalReferences.Scripts;
using Features.Loot;
using Features.Loot.Scripts;
using Features.Unit.Modding;
using Features.Unit.Modding.Stat;
using Plugins.UniRx.Extensions;
using UniRx;
using UnityEngine;

namespace Features.Battle.Scripts
{
    [CreateAssetMenu]
    public class BattleData_SO : ScriptableObject
    {
        [SerializeField] private NetworkedUnitRuntimeSet_SO enemyUnitRuntimeSet;

        [SerializeField] private NetworkedUnitRuntimeSet_SO playerTeamUnitRuntimeSet;
        
        [SerializeField] private IntReactiveVariable stage;

        public NetworkedUnitRuntimeSet_SO EnemyUnitRuntimeSet => enemyUnitRuntimeSet;
        
        public NetworkedUnitRuntimeSet_SO PlayerTeamUnitRuntimeSet => playerTeamUnitRuntimeSet;

        public BattleManager BattleManager { get; private set; }
        public IState CurrentState => BattleManager.CurrentState;

        public List<LootableGenerator_SO> lootables;

        public void RegisterBattleManager(BattleManager battleManager)
        {
            lootables = new List<LootableGenerator_SO>();
            BattleManager = battleManager;
        }

        public void SetAiStats(AIUnitBehaviour aiUnitBehaviour)
        {
            if (aiUnitBehaviour.NetworkingInitialized)
            {
                aiUnitBehaviour.NetworkedStatServiceLocator.SetBaseValue(StatType.Damage, 10 * (stage.Get()));
                aiUnitBehaviour.NetworkedStatServiceLocator.SetBaseValue(StatType.Health, 50 * (stage.Get()));
                aiUnitBehaviour.NetworkedStatServiceLocator.SetBaseValue(StatType.Speed, 3);
            }
        }
    }
}
