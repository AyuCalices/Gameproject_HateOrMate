using System;
using System.Collections.Generic;
using Features.Unit;
using Features.Unit.Modding.Stat;
using UnityEngine;

namespace Features.Mod
{
    public enum UnitOwnerType { LocalPlayer, ExternPlayer }

    [CreateAssetMenu(fileName = "new MultipleStatMod", menuName = "Unit/Mod/MultipleStat")]
    public class MultipleStatModGenerator_SO : BaseModGenerator_SO
    {
        [SerializeField] private List<MultipleStatModTarget> multipleStatModTargets;


        public override BaseMod Generate()
        {
            return new MultipleStatMod(multipleStatModTargets, ModName, Description);
        }
    }

    [Serializable]
    public class MultipleStatModTarget
    {
        public UnitOwnerType unitOwnerType;
        public StatType statType;
        public float baseValue;
        public float scaleValue;
    }
}