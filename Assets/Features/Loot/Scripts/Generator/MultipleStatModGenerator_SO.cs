using System;
using System.Collections.Generic;
using DataStructures.ReactiveVariable;
using Features.BattleScene.Scripts;
using Features.Mods.Scripts.ModTypes;
using Features.Mods.Scripts.View;
using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Services.BattleBehaviour;
using Features.Unit.Scripts.Behaviours.Services.UnitStats;
using UnityEngine;
using UnityEngine.Serialization;

namespace Features.Loot.Scripts.Generator
{
    [CreateAssetMenu(fileName = "new MultipleStatMod", menuName = "Unit/Mod/MultipleStat")]
    public class MultipleStatModGenerator_SO : LootableGenerator_SO
    {
        [FormerlySerializedAs("modBehaviourPrefab")] [SerializeField] private ModViewBehaviour modViewBehaviourPrefab;
        [SerializeField] private List<MultipleStatModTarget> multipleStatModTargets;
        [SerializeField] private BattleData_SO battleData;


        public override void OnAddInstanceToPlayer(int stageAsLevel)
        {
            MultipleStatMod multipleStatMod = new MultipleStatMod(multipleStatModTargets, battleData, SpritePrefab, Description, stageAsLevel, modViewBehaviourPrefab);
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