using System.Collections.Generic;
using System.Linq;
using DataStructures.ReactiveVariable;
using DataStructures.StateLogic;
using Features.Loot.Scripts;
using Features.Tiles;
using Features.Tiles.Scripts;
using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Battle;
using UnityEngine;

namespace Features.Battle.Scripts
{
    public enum TeamType{Own, MateElseEnemy, Enemy}
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

        public NetworkedUnitRuntimeSet_SO NetworkedUnitRuntimeSet => networkedUnitRuntimeSet;
        [SerializeField] private NetworkedUnitRuntimeSet_SO networkedUnitRuntimeSet;
        
        public NetworkedUnitRuntimeSet_SO LocalUnitRuntimeSet => localUnitRuntimeSet;
        [SerializeField] private NetworkedUnitRuntimeSet_SO localUnitRuntimeSet;
        
        public TileRuntimeDictionary_SO TileRuntimeDictionary => tileRuntimeDictionary;
        [SerializeField] private TileRuntimeDictionary_SO tileRuntimeDictionary;
        
        public IntReactiveVariable Stage => stage;
        [SerializeField] private IntReactiveVariable stage;

        public LootTable_SO LootTable => lootTable;
        [SerializeField] private LootTable_SO lootTable;
        
        
        public IState CurrentState => _battleManager.CurrentState;
        private BattleManager _battleManager;

        public NetworkedUnitRuntimeSet_SO GetEnemyTeam(NetworkedUnitRuntimeSet_SO ownTeamRuntimeSet)
        {
            if (ownTeamRuntimeSet == enemyUnitsRuntimeSet)
            {
                return playerUnitsRuntimeSet;
            }
            if (ownTeamRuntimeSet == playerUnitsRuntimeSet)
            {
                return enemyUnitsRuntimeSet;
            }
            
            Debug.LogError("Please pass a valid Team!");
            return null;
        }
        
        public List<NetworkedBattleBehaviour> GetTeam(NetworkedUnitRuntimeSet_SO ownTeamRuntimeSet, TeamType teamType)
        {
            switch (teamType)
            {
                case TeamType.Own:
                    return ownTeamRuntimeSet.GetItems();
                case TeamType.MateElseEnemy:
                    List<NetworkedBattleBehaviour> mateUnitList = new List<NetworkedBattleBehaviour>();
                    foreach (NetworkedBattleBehaviour enemyUnit in GetEnemyTeam(ownTeamRuntimeSet).GetItems())
                    {
                        if (!NetworkedUnitRuntimeSet.GetItems().Contains(enemyUnit))
                        {
                            mateUnitList.Add(enemyUnit);
                        }
                    }
                    return mateUnitList.Count == 0 ? GetEnemyTeam(ownTeamRuntimeSet).GetItems() : mateUnitList;
                case TeamType.Enemy:
                    return GetEnemyTeam(ownTeamRuntimeSet).GetItems();
            }
            
            return null;
        }
        

        public void RegisterBattleManager(BattleManager battleManager)
        {
            _battleManager = battleManager;
        }
    }
}
