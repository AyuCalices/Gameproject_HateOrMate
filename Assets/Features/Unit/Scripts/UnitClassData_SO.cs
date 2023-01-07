using Features.Connection;
using Features.Connection.Scripts.Utils;
using Features.Unit.Scripts.ClassGenerator;
using UnityEngine;

namespace Features.Unit.Scripts
{
    [CreateAssetMenu]
    public class UnitClassData_SO : NetworkedScriptableObject
    {
        public UnitType_SO unitType;
        public BattleClassGenerator_SO battleClasses;
        public Sprite sprite;
        public float range;
        public float movementSpeed;
    }
}
