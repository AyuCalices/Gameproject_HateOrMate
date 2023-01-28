using UnityEngine;

namespace Features.Battle.Scripts.StageProgression
{
    [CreateAssetMenu(fileName = "BaseStats", menuName = "StageProgression/BaseStats")]
    public class BaseStatsData_SO : ScriptableObject
    {
        [Header("Attack")]
        public float attackBaseValue = 10f;
        public float attackMinValue = 10f;
        
        [Header("Health")]
        public float healthBaseValue = 50f;
        public float healthMinValue = 50f;

        [Header("Range")]
        public float rangeValue = 2f;
        public float rangeMinValue = 1f;
    
        [Header("MovementSpeed")]
        public float movementSpeedValue = 3f;
        public float movementSpeedMinValue = 1f;
        
        [Header("Speed")] 
        public float speedValue = 3f;
        public float speedMinValue = 1f;
    
        [Header("Stamina (only for Towers)")]
        public float staminaValue = 10f;
        public float staminaMinValue = 3f;
    }
}
