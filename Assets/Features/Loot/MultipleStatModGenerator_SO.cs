using System;
using System.Collections.Generic;
using Features.GlobalReferences;
using Features.Mod;
using Features.ModView;
using Features.Unit.Modding.Stat;
using UnityEngine;

namespace Features.Loot
{
    public enum UnitOwnerType { LocalPlayer, ExternPlayer }

    [CreateAssetMenu(fileName = "new MultipleStatMod", menuName = "Unit/Mod/MultipleStat")]
    public class MultipleStatModGenerator_SO : LootableGenerator_SO
    {
        [SerializeField] private ModDragBehaviour modDragBehaviourPrefab;
        [SerializeField] private NetworkedUnitRuntimeSet_SO localUnitRuntimeSet;
        [SerializeField] private List<MultipleStatModTarget> multipleStatModTargets;


        public override void OnAddInstanceToPlayer()
        {
            localUnitRuntimeSet.TryInstantiateModToAny(modDragBehaviourPrefab,
                new MultipleStatMod(multipleStatModTargets, LootableName, Description));
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