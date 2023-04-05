using Features.General.Photon.Scripts;
using Features.Unit.Scripts.Behaviours.Services.UnitStats;
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
        public BaseDamageAnimationBehaviour baseDamageAnimationBehaviour;
        public BaseStatsData_SO baseStatsData;
        public Sprite sprite;
        public bool levelUpOnStageClear;
    }
}
