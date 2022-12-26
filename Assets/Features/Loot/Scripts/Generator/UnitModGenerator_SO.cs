using Features.Loot.Scripts.GeneratedLoot;
using Features.Loot.Scripts.ModView;
using Features.Unit.Scripts;
using Features.Unit.Scripts.Behaviours;
using UnityEngine;

namespace Features.Loot.Scripts.Generator
{
    [CreateAssetMenu(fileName = "new UnitMod", menuName = "Unit/Mod/Unit")]
    public class UnitModGenerator_SO : LootableGenerator_SO
    {
        [SerializeField] private NetworkedUnitRuntimeSet_SO localUnitRuntimeSet;
        [SerializeField] private ModDragBehaviour modDragBehaviourPrefab;
        [SerializeField] private UnitClassData_SO classData;
        
        public override void OnAddInstanceToPlayer()
        {
            localUnitRuntimeSet.TryInstantiateModToAny(modDragBehaviourPrefab, new UnitMod(classData, LootableName, Description));
        }
    }
}
