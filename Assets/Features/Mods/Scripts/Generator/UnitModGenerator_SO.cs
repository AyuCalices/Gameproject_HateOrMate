using DataStructures.ReactiveVariable;
using Features.Loot.Scripts.GeneratedLoot;
using Features.Loot.Scripts.ModView;
using Features.Unit.Scripts;
using Features.Unit.Scripts.Behaviours.Mod;
using UnityEngine;
using UnityEngine.Serialization;

namespace Features.Loot.Scripts.Generator
{
    [CreateAssetMenu(fileName = "new UnitMod", menuName = "Unit/Mod/Unit")]
    public class UnitModGenerator_SO : LootableGenerator_SO
    {
        [SerializeField] private ModBehaviour modBehaviourPrefab;
        [SerializeField] private UnitClassData_SO classData;
        [FormerlySerializedAs("unitViewRuntimeSet")] [SerializeField] private UnitDisplayRuntimeSet_SO unitDisplayRuntimeSet;
        
        public override void OnAddInstanceToPlayer(int stageAsLevel)
        {
            UnitMod unitMod = new UnitMod(classData, unitDisplayRuntimeSet, SpritePrefab, Description, stageAsLevel, modBehaviourPrefab);
            unitMod.RaiseOnModInstantiated();
        }
    }
}
