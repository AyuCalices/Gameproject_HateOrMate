using Features.Unit;
using Features.Unit.Stat;
using UnityEngine;

namespace Features.Mod
{
    [CreateAssetMenu(fileName = "new SingleStatMod", menuName = "Unit/Mod/SingleStat")]
    public class SingleStatModGeneratorGeneratorSo : BaseModGenerator_SO
    {
        [SerializeField] private StatType statType;
        [SerializeField] private float baseValue;
        [SerializeField] private float scaleValue;


        public override BaseMod Generate()
        {
            return new SingleStatMod(statType, baseValue, scaleValue, ModName, Description);
        }
    }
}