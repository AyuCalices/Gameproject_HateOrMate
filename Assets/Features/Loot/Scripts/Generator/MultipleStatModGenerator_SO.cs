using System;
using System.Collections.Generic;
using DataStructures.ReactiveVariable;
using Features.Battle.Scripts;
using Features.Loot.Scripts.GeneratedLoot;
using Features.Loot.Scripts.ModView;
using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Stats;
using UnityEngine;

namespace Features.Loot.Scripts.Generator
{
    [CreateAssetMenu(fileName = "new MultipleStatMod", menuName = "Unit/Mod/MultipleStat")]
    public class MultipleStatModGenerator_SO : LootableGenerator_SO
    {
        [SerializeField] private ModBehaviour modBehaviourPrefab;
        [SerializeField] private List<MultipleStatModTarget> multipleStatModTargets;
        [SerializeField] private BattleData_SO battleData;


        public override void OnAddInstanceToPlayer(int stageAsLevel)
        {
            MultipleStatMod multipleStatMod = new MultipleStatMod(multipleStatModTargets, battleData, SpritePrefab, Description, stageAsLevel, modBehaviourPrefab);
            multipleStatMod.RaiseOnModInstantiated();
        }
    }

    [Serializable]
    public class MultipleStatModTarget
    {
        public TeamTagType[] teamTagType;
        public StatType statType;
        public float baseValue;
        public float scaleValue;
        public float stageScaleValue;
    }
}