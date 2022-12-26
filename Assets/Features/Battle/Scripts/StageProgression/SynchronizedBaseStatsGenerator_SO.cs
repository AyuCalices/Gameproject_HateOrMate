using DataStructures.ReactiveVariable;
using Features.Unit.Scripts.Behaviours.Stat;
using UnityEngine;

namespace Features.Battle.Scripts.StageProgression
{
    //TODO: IGenerate
    [CreateAssetMenu(fileName = "SynchronizedBaseStatsGenerator", menuName = "StageProgression/SynchronizedBaseStatsGenerator")]
    public class SynchronizedBaseStatsGenerator_SO : ScriptableObject
    {
        [SerializeField] private IntReactiveVariable currentStage;
        
        [Header("Attack")]
        [SerializeField] private float attackBaseValue;
        [SerializeField] private float attackScaling;
        
        [Header("Health")]
        [SerializeField] private float healthBaseValue;
        [SerializeField] private float healthScaling;
        
        [Header("Speed")]
        [SerializeField] private float speedValue;
        
        [Header("Stamina")]
        [SerializeField] private float staminaValue;
        [SerializeField] private float staminaRefreshTime;
        
        public BaseStats GetSynchronizedStats()
        {
            float finalAttack = attackBaseValue * Mathf.Pow(attackScaling, currentStage.Get());
            float finalHealth = healthBaseValue * Mathf.Pow(healthScaling, currentStage.Get());

            return new BaseStats(finalAttack, finalHealth, speedValue);
        }
    }
}
