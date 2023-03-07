using Features.Mods.Scripts.ModTypes;
using Features.Mods.Scripts.View;
using Features.Unit.Scripts.Behaviours.Services.UnitStats;
using UnityEngine;
using UnityEngine.Serialization;

namespace Features.Loot.Scripts.Generator
{
    [CreateAssetMenu(fileName = "new SingleStatMod", menuName = "Unit/Mod/SingleStat")]
    public class SingleStatModGenerator_SO : LootableGenerator_SO
    {
        [FormerlySerializedAs("modBehaviourPrefab")] [SerializeField] private ModViewBehaviour modViewBehaviourPrefab;
        [SerializeField] private StatType statType;
        [SerializeField] private float baseValue;
        [SerializeField] private float scaleValue;
        [SerializeField] private float stageScaleValue;
        
        public override void OnAddInstanceToPlayer(int stageAsLevel)
        {
            SingleStatMod singleStatMod = new SingleStatMod(statType, baseValue, scaleValue, stageScaleValue, SpritePrefab, Description, stageAsLevel, modViewBehaviourPrefab);
            singleStatMod.RaiseOnModInstantiated();
        }
    }
}