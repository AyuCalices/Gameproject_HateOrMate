using System;
using System.Collections.Generic;
using DataStructures.ReactiveVariable;
using Features.BattleScene.Scripts.StateMachine;
using Features.Loot.Scripts;
using Features.Loot.Scripts.Generator;
using Features.Tiles.Scripts;
using Features.Unit.Scripts.Behaviours;
using UnityEngine;
using UnityEngine.Serialization;

namespace Features.BattleScene.Scripts
{
    /// <summary>
    /// Container of all the battle relevant data. By exchanging an instance of this, you can configure a new battle (different Tile Set, Enemy & Player)
    /// </summary>
    [CreateAssetMenu]
    public class BattleData_SO : ScriptableObject
    {
        public UnitServiceProviderRuntimeSet_SO UnitsServiceProviderRuntimeSet => unitServiceProviderRuntimeSet;
        [FormerlySerializedAs("allUnitsRuntimeSet")] [SerializeField] private UnitServiceProviderRuntimeSet_SO unitServiceProviderRuntimeSet;
        
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
