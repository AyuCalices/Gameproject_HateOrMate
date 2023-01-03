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
        [SerializeField] private ModBehaviour modBehaviourPrefab;
        [SerializeField] private UnitClassData_SO classData;
        
        public override void OnAddInstanceToPlayer()
        {
            UnitMod unitMod = new UnitMod(classData, LootableName, Description, modBehaviourPrefab);
            unitMod.RaiseOnModInstantiated();
        }
    }
}
