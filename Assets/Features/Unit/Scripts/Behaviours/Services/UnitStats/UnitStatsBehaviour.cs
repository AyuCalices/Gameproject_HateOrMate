using System;
using Features.Mods.Scripts.View;
using Features.Unit.Scripts.Behaviours.Services.UnitStats.StatTypes;
using JetBrains.Annotations;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

namespace Features.Unit.Scripts.Behaviours.Services.UnitStats
{
    [RequireComponent(typeof(PhotonView))]
    public class UnitStatsBehaviour : MonoBehaviourPunCallbacks
    {
        [FormerlySerializedAs("unitViewRuntimeSet")] [SerializeField] private UnitDisplayRuntimeSet_SO unitDisplayRuntimeSet;
        
        public StatServiceLocator StatServiceLocator { get; private set; }
        public float RemovedHealth { get; set; }
        
        
        private UnitServiceProvider _unitServiceProvider;
        
        
        public void Initialize(UnitServiceProvider unitServiceProvider, BaseStatsData_SO baseStatsData, int level)
        {
            _unitServiceProvider = unitServiceProvider;
            StatServiceLocator = new StatServiceLocator();

            InitializeStatsForClients();
            SetBaseStats(baseStatsData, level);
            ApplyModToInstantiatedUnit();
        }

        private void OnDestroy()
        {
            StatServiceLocator.UnregisterAll();
        }
        
        public float GetFinalStat(StatType statType)
        {
            float totalBaseValue = StatServiceLocator.GetTotalBaseValue(statType);
            float totalMultiplierValue = StatServiceLocator.GetTotalMultiplierValue(statType);

            if (StatServiceLocator.TryGetService(out BaseStat baseStat, statType))
            {
                return Mathf.Max(totalBaseValue * Mathf.Max(totalMultiplierValue, baseStat.GetMultiplierMinValue()), baseStat.GetBaseMinValue());
            }
            
            Debug.LogWarning($"There is no {typeof(BaseStat)} Registered inside the {StatServiceLocator}");
            return totalBaseValue;
        }
        
        public void SetBaseStats(BaseStatsData_SO baseStatsData, int level)
        {
            float finalAttack = baseStatsData.attackBaseValue * Mathf.Pow(baseStatsData.attackLevelScaling, level);
            float finalHealth = baseStatsData.healthBaseValue * Mathf.Pow(baseStatsData.healthLevelScaling, level);

            //TODO: find different system
            StatServiceLocator.TrySetStatValue<BaseStat>(StatType.Damage, StatValueType.Stat, finalAttack);
            StatServiceLocator.TrySetStatValue<BaseStat>(StatType.Damage, StatValueType.ScalingStat, baseStatsData.attackMultiplierValue);
            StatServiceLocator.TrySetStatValue<BaseStat>(StatType.Damage, StatValueType.MinStat, baseStatsData.attackMinValue);
            StatServiceLocator.TrySetStatValue<BaseStat>(StatType.Damage, StatValueType.MinScalingStat, baseStatsData.attackMultiplierMinValue);
            
            StatServiceLocator.TrySetStatValue<BaseStat>(StatType.Health, StatValueType.Stat, finalHealth);
            StatServiceLocator.TrySetStatValue<BaseStat>(StatType.Health, StatValueType.ScalingStat, baseStatsData.healthMultiplierValue);
            StatServiceLocator.TrySetStatValue<BaseStat>(StatType.Health, StatValueType.MinStat, baseStatsData.healthMinValue);
            StatServiceLocator.TrySetStatValue<BaseStat>(StatType.Health, StatValueType.MinScalingStat, baseStatsData.healthMultiplierMinValue);
            
            StatServiceLocator.TrySetStatValue<BaseStat>(StatType.Speed, StatValueType.Stat, baseStatsData.speedValue);
            StatServiceLocator.TrySetStatValue<BaseStat>(StatType.Speed, StatValueType.MinStat, baseStatsData.speedMinValue);
            
            StatServiceLocator.TrySetStatValue<BaseStat>(StatType.Range, StatValueType.Stat, baseStatsData.rangeValue);
            StatServiceLocator.TrySetStatValue<BaseStat>(StatType.Range, StatValueType.MinStat, baseStatsData.rangeMinValue);
            
            StatServiceLocator.TrySetStatValue<BaseStat>(StatType.MovementSpeed, StatValueType.Stat, baseStatsData.movementSpeedValue);
            StatServiceLocator.TrySetStatValue<BaseStat>(StatType.MovementSpeed, StatValueType.MinStat, baseStatsData.movementSpeedMinValue);
            
            StatServiceLocator.TrySetStatValue<BaseStat>(StatType.Stamina, StatValueType.Stat, baseStatsData.staminaValue);
            StatServiceLocator.TrySetStatValue<BaseStat>(StatType.Stamina, StatValueType.MinStat, baseStatsData.staminaMinValue);
        }
        
        private void ApplyModToInstantiatedUnit()
        {
            foreach (UnitDisplayBehaviour unitModBehaviour in unitDisplayRuntimeSet.GetItems())
            {
                unitModBehaviour.UnitDisplayMods.ApplyToInstantiatedUnit(_unitServiceProvider);
            }
        }
        
        private void InitializeStatsForClients()
        {
            Array statTypeValues = Enum.GetValues(typeof(StatType));

            int[] enumValues = new int[statTypeValues.Length];
            string[] scalingStatIdentities = new string[statTypeValues.Length];
            string[] statIdentities = new string[statTypeValues.Length];

            int index = 0;
            foreach (object value in statTypeValues)
            {
                string scalingStatIdentity = Guid.NewGuid().ToString();
                string statIdentity = Guid.NewGuid().ToString();
                StatServiceLocator.Register(new LocalModificationStat((StatType)value, scalingStatIdentity, statIdentity));
                StatServiceLocator.Register(new BaseStat((StatType)value));

                enumValues[index] = (int) value;
                scalingStatIdentities[index] = scalingStatIdentity;
                statIdentities[index] = statIdentity;
                index++;
                
            }

            if (_unitServiceProvider.GetService<PhotonView>().ViewID != 0)
            {
                _unitServiceProvider.GetService<PhotonView>().RPC("SynchNetworkStat", RpcTarget.Others, enumValues, scalingStatIdentities, statIdentities);
            }
            else
            {
                Debug.LogError("View ID Not Allocated");
            }
        }
        
        [PunRPC, UsedImplicitly]
        protected void SynchNetworkStat(int[] statType, string[] scalingStatIdentity, string[] statIdentity)
        {
            for (var i = 0; i < statType.Length; i++)
            {
                StatServiceLocator.Register(new NetworkModificationStat((StatType)statType[i], scalingStatIdentity[i], statIdentity[i]));
            }
        }
    }
}
