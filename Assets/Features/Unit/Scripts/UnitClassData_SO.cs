using Features.Connection;
using Features.Unit.Scripts.ClassGenerator;
using UnityEngine;

namespace Features.Unit.Scripts
{
    [CreateAssetMenu]
    public class UnitClassData_SO : NetworkedScriptableObject
    {
        public BattleClassGenerator_SO battleClasses;
        public float range;
        public float movementSpeed;
    }
}
