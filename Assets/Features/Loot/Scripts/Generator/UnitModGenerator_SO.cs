using DataStructures.ReactiveVariable;
using Features.Mods.Scripts.ModTypes;
using Features.Mods.Scripts.View;
using Features.Unit.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace Features.Loot.Scripts.Generator
{
    [CreateAssetMenu(fileName = "new UnitMod", menuName = "Unit/Mod/Unit")]
    public class UnitModGenerator_SO : LootableGenerator_SO
    {
        [FormerlySerializedAs("modBehaviourPrefab")] [SerializeField] private ModViewBehaviour modViewBehaviourPrefab;
        [SerializeField] private UnitClassData_SO classData;
        [FormerlySerializedAs("unitViewRuntimeSet")] [SerializeField] private UnitDisplayRuntimeSet_SO unitDisplayRuntimeSet;
        
        public override void OnAddInstanceToPlayer(int stageAsLevel)
        {
            UnitMod unitMod = new UnitMod(classData, unitDisplayRuntimeSet, SpritePrefab, Description, stageAsLevel, modViewBehaviourPrefab);
            unitMod.RaiseOnModInstantiated();
        }
    }
}
