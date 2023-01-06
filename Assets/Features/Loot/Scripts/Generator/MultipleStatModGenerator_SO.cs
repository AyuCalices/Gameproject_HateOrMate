using System;
using System.Collections.Generic;
using Features.Battle.Scripts;
using Features.Loot.Scripts.GeneratedLoot;
using Features.Loot.Scripts.ModView;
using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Stat;
using UnityEngine;

namespace Features.Loot.Scripts.Generator
{
    [CreateAssetMenu(fileName = "new MultipleStatMod", menuName = "Unit/Mod/MultipleStat")]
    public class MultipleStatModGenerator_SO : LootableGenerator_SO
    {
        [SerializeField] private ModBehaviour modBehaviourPrefab;
        [SerializeField] private List<MultipleStatModTarget> multipleStatModTargets;
        [SerializeField] private NetworkedUnitRuntimeSet_SO ownTeam;
        [SerializeField] private BattleData_SO battleData;


        public override void OnAddInstanceToPlayer()
        {
            MultipleStatMod multipleStatMod = new MultipleStatMod(multipleStatModTargets, battleData, ownTeam, SpritePrefab, Description, modBehaviourPrefab);
            multipleStatMod.RaiseOnModInstantiated();
        }
    }

    [Serializable]
    public class MultipleStatModTarget
    {
        public TeamType teamType;
        public StatType statType;
        public float baseValue;
        public float scaleValue;
    }
}