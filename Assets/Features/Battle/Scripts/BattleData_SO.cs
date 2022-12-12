using System.Collections.Generic;
using DataStructures.ReactiveVariable;
using DataStructures.StateLogic;
using Features.GlobalReferences.Scripts;
using Features.Loot.Scripts;
using Features.Tiles;
using Features.Unit.Modding;
using Features.Unit.Modding.Stat;
using UnityEngine;

namespace Features.Battle.Scripts
{
    /// <summary>
    /// Container of all the battle relevant data. By exchanging an instance of this, you can configure a new battle (different Tile Set, Enemy & Player)
    /// </summary>
    [CreateAssetMenu]
    public class BattleData_SO : ScriptableObject
    {
        [SerializeField] private NetworkedUnitRuntimeSet_SO enemyUnitRuntimeSet;

        [SerializeField] private NetworkedUnitRuntimeSet_SO playerTeamUnitRuntimeSet;
        
        [SerializeField] private IntReactiveVariable stage;

        [SerializeField] private TileRuntimeDictionary_SO tileRuntimeDictionary;
        
        public NetworkedUnitRuntimeSet_SO EnemyUnitRuntimeSet => enemyUnitRuntimeSet;
        
        public NetworkedUnitRuntimeSet_SO PlayerTeamUnitRuntimeSet => playerTeamUnitRuntimeSet;

        public TileRuntimeDictionary_SO TileRuntimeDictionary => tileRuntimeDictionary;

        
        private BattleManager _battleManager;
        public IState CurrentState => _battleManager.CurrentState;
        

        public List<LootableGenerator_SO> lootables;

        public void RegisterBattleManager(BattleManager battleManager)
        {
            lootables = new List<LootableGenerator_SO>();
            _battleManager = battleManager;
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
