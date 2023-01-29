using Features.Battle.Scripts.StageProgression;
using Features.Connection.Scripts.Utils;
using Features.Unit.Scripts.ClassGenerator;
using Features.Unit.Scripts.DamageAnimation;
using UnityEngine;

namespace Features.Unit.Scripts
{
    [CreateAssetMenu]
    public class UnitClassData_SO : NetworkedScriptableObject
    {
        public UnitType_SO unitType;
        public BattleClassGenerator_SO battleClasses;
        public BaseStatsGenerator_SO baseStatsGenerator;
        public BaseDamageAnimationBehaviour baseDamageAnimationBehaviour;
        public BaseStatsData_SO baseStatsData;
        public Sprite sprite;
    }
}
