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
        public NetworkedUnitRuntimeSet_SO EnemyUnitsRuntimeSet => enemyUnitsRuntimeSet;
        [SerializeField] private NetworkedUnitRuntimeSet_SO enemyUnitsRuntimeSet;

        public NetworkedUnitRuntimeSet_SO PlayerUnitsRuntimeSet => playerUnitsRuntimeSet;
        [SerializeField] private NetworkedUnitRuntimeSet_SO playerUnitsRuntimeSet;

        public NetworkedUnitRuntimeSet_SO AllUnitsRuntimeSet => allUnitsRuntimeSet;
        [SerializeField] private NetworkedUnitRuntimeSet_SO allUnitsRuntimeSet;

        public TileRuntimeDictionary_SO TileRuntimeDictionary => tileRuntimeDictionary;
        [SerializeField] private TileRuntimeDictionary_SO tileRuntimeDictionary;
        
        public IntReactiveVariable Stage => stage;
        [SerializeField] private IntReactiveVariable stage;

        public LootTable_SO LootTable => lootTable;
        [SerializeField] private LootTable_SO lootTable;
        
        
        public IState CurrentState => _battleManager.CurrentState;
        private BattleManager _battleManager;
        

        public void RegisterBattleManager(BattleManager battleManager)
        {
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
