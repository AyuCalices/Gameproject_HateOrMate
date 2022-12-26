using Features.Loot.Scripts.GeneratedLoot;
using Features.Loot.Scripts.ModView;
using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Stat;
using UnityEngine;

namespace Features.Loot.Scripts.Generator
{
    [CreateAssetMenu(fileName = "new SingleStatMod", menuName = "Unit/Mod/SingleStat")]
    public class SingleStatModGenerator_SO : LootableGenerator_SO
    {
        [SerializeField] private NetworkedUnitRuntimeSet_SO localUnitRuntimeSet;
        [SerializeField] private ModDragBehaviour modDragBehaviourPrefab;
        [SerializeField] private StatType statType;
        [SerializeField] private float baseValue;
        [SerializeField] private float scaleValue;
        
        public override void OnAddInstanceToPlayer()
        {
            localUnitRuntimeSet.TryInstantiateModToAny(modDragBehaviourPrefab, new SingleStatMod(statType, baseValue, scaleValue, LootableName, Description));
        }
    }
}