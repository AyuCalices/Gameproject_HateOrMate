using System;
using System.Collections.Generic;
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
        [SerializeField] private ModDragBehaviour modDragBehaviourPrefab;
        [SerializeField] private List<MultipleStatModTarget> multipleStatModTargets;


        public override void OnAddInstanceToPlayer()
        {
            MultipleStatMod multipleStatMod = new MultipleStatMod(multipleStatModTargets, LootableName, Description, modDragBehaviourPrefab);
            multipleStatMod.RaiseOnModInstantiated();
        }
    }

    [Serializable]
    public class MultipleStatModTarget
    {
        public NetworkedUnitRuntimeSet_SO networkedUnitRuntimeSet;
        public StatType statType;
        public float baseValue;
        public float scaleValue;
    }
}