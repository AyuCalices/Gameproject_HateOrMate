using Features.GlobalReferences.Scripts;
using Features.Mod;
using Features.ModView;
using Features.Unit.Classes;
using UnityEngine;

namespace Features.Loot.Scripts
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
