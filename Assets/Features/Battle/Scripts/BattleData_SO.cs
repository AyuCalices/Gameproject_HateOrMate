using System;
using System.Collections.Generic;
using System.Linq;
using DataStructures.ReactiveVariable;
using DataStructures.StateLogic;
using Features.Battle.StateMachine;
using Features.Loot.Scripts;
using Features.Loot.Scripts.Generator;
using Features.Tiles;
using Features.Tiles.Scripts;
using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Battle;
using UnityEngine;

namespace Features.Battle.Scripts
{
    /// <summary>
    /// Container of all the battle relevant data. By exchanging an instance of this, you can configure a new battle (different Tile Set, Enemy & Player)
    /// </summary>
    [CreateAssetMenu]
    public class BattleData_SO : ScriptableObject
    {
        public NetworkedUnitRuntimeSet_SO AllUnitsRuntimeSet => allUnitsRuntimeSet;
        [SerializeField] private NetworkedUnitRuntimeSet_SO allUnitsRuntimeSet;
        
        public TileRuntimeDictionary_SO TileRuntimeDictionary => tileRuntimeDictionary;
        [SerializeField] private TileRuntimeDictionary_SO tileRuntimeDictionary;
        
        public IntReactiveVariable Stage => stage;
        [SerializeField] private IntReactiveVariable stage;

        public IntReactiveVariable CompletedStage => completedStage;
        [SerializeField] private IntReactiveVariable completedStage;
        
        public LootTable_SO LootTable => lootTable;
        [SerializeField] private LootTable_SO lootTable;
        
        public bool IsStageRestart { get; set; }
        public List<LootableGenerator_SO> Lootables { get; set; }
        public List<int> LootableStages { get; set; }
        
        private BattleManager _battleManager;
        
        public bool StateIsValid(Type checkedType, StateProgressType checkedStateProgressType)
        {
            return _battleManager.StateIsValid(checkedType, checkedStateProgressType);
        }
        
        public void Initialize(BattleManager battleManager)
        {
            _battleManager = battleManager;
            IsStageRestart = false;
            Stage.Restore(true);

            Lootables = new List<LootableGenerator_SO>();
            LootableStages = new List<int>();
        }
    }
}
