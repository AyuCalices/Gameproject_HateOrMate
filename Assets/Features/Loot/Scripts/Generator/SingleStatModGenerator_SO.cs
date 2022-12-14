using Features.Loot.Scripts.GeneratedLoot;
using Features.Loot.Scripts.ModView;
using Features.Unit.Scripts.Behaviours.Stat;
using UnityEngine;

namespace Features.Loot.Scripts.Generator
{
    [CreateAssetMenu(fileName = "new SingleStatMod", menuName = "Unit/Mod/SingleStat")]
    public class SingleStatModGenerator_SO : LootableGenerator_SO
    {
        [SerializeField] private ModBehaviour modBehaviourPrefab;
        [SerializeField] private StatType statType;
        [SerializeField] private float baseValue;
        [SerializeField] private float scaleValue;
        [SerializeField] private float stageScaleValue;
        
        public override void OnAddInstanceToPlayer(int stageAsLevel)
        {
            SingleStatMod singleStatMod = new SingleStatMod(statType, baseValue, scaleValue, stageScaleValue, SpritePrefab, Description, stageAsLevel, modBehaviourPrefab);
            singleStatMod.RaiseOnModInstantiated();
        }
    }
}