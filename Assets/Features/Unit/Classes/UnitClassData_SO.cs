using Features.Mod.Action;
using UnityEngine;

namespace Features.Unit.Classes
{
    [CreateAssetMenu]
    public class UnitClassData_SO : NetworkedScriptableObject
    {
        public BattleActionGenerator_SO battleActions;
        public float range;
        public float movementSpeed;
    }
}
