using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Class;
using Features.Unit.Scripts.DamageAnimation;
using UnityEngine;

namespace Features.Unit.Scripts.ClassGenerator
{
    [CreateAssetMenu(fileName = "new AiTowerBattleActions", menuName = "Unit/Actions/AiTowerBattleActions")]
    public class CasterBattleClassGenerator : BattleClassGenerator_SO
    {
        [SerializeField] private bool isAI;
        
        protected override BattleClass InternalGenerate(UnitServiceProvider ownerUnitServiceProvider, BaseDamageAnimationBehaviour baseDamageAnimationBehaviour)
        {
            return new CasterBattleClass(isAI, ownerUnitServiceProvider, baseDamageAnimationBehaviour);
        }
    }
}
