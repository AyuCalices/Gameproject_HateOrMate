using UnityEngine;

namespace Features.Battle.Scripts.StageProgression
{
    [CreateAssetMenu(fileName = "BaseStats", menuName = "StageProgression/BaseStats")]
    public class BaseStatsData_SO : ScriptableObject
    {
        [Header("Attack")]
        public float attackBaseValue = 10f;
        public float attackMultiplier = 1f;
        public float attackMinValue = 10f;
        public float attackLevelScaling = 1.1f;

        [Header("Health")]
        public float healthBaseValue = 50f;
        public float healthMultiplier = 1f;
        public float healthMinValue = 50f;
        public float healthLevelScaling = 1.1f;

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

        [Header("Balancing: Attack - Health - Speed")] 
        public float orientationHealth = 60f;
        public float orientationKillTime = 15f;
        public int expectedEnemyClientSize = 1;
    }
}
