using System;
using Features.Loot.Scripts;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Behaviours.Mod;
using Features.Unit.Scripts.Stats;
using JetBrains.Annotations;
using Photon.Pun;
using UnityEngine;

namespace Features.Unit.Scripts.Behaviours
{
    [RequireComponent(typeof(PhotonView))]
    public class NetworkedStatsBehaviour : MonoBehaviourPunCallbacks
    {
        [SerializeField] private UnitViewRuntimeSet_SO unitViewRuntimeSet;
        
        public NetworkedStatServiceLocator NetworkedStatServiceLocator { get; private set; }
        public float RemovedHealth { get; set; }
        
        
        private UnitServiceProvider _unitServiceProvider;
        
        
        public void Initialize(UnitServiceProvider unitServiceProvider, BaseStatsData_SO baseStatsData, int level)
        {
            _unitServiceProvider = unitServiceProvider;
            NetworkedStatServiceLocator = new NetworkedStatServiceLocator();

            InitializeStatsForClients();
            SetBaseStats(baseStatsData, level);
            ApplyModToInstantiatedUnit();
        }

        private void OnDestroy()
        {
            NetworkedStatServiceLocator.UnregisterAll();
        }
        
        public float GetFinalStat(StatType statType)
        {
            float totalBaseValue = NetworkedStatServiceLocator.GetTotalBaseValue(statType);
            float totalMultiplierValue = NetworkedStatServiceLocator.GetTotalMultiplierValue(statType);

            if (NetworkedStatServiceLocator.TryGetService(out BaseStat baseStat, statType))
            {
                return Mathf.Max(totalBaseValue * Mathf.Max(totalMultiplierValue, baseStat.GetMultiplierMinValue()), baseStat.GetBaseMinValue());
            }
            
            Debug.LogWarning($"There is no {typeof(BaseStat)} Registered inside the {NetworkedStatServiceLocator}");
            return totalBaseValue;
        }
        
        public void SetBaseStats(BaseStatsData_SO baseStatsData, int level)
        {
            float finalAttack = baseStatsData.attackBaseValue * Mathf.Pow(baseStatsData.attackLevelScaling, level);
            float finalHealth = baseStatsData.healthBaseValue * Mathf.Pow(baseStatsData.healthLevelScaling, level);

            //TODO: find different system
            NetworkedStatServiceLocator.TrySetStatValue<BaseStat>(StatType.Damage, StatValueType.Stat, finalAttack);
            NetworkedStatServiceLocator.TrySetStatValue<BaseStat>(StatType.Damage, StatValueType.ScalingStat, baseStatsData.attackMultiplierValue);
            NetworkedStatServiceLocator.TrySetStatValue<BaseStat>(StatType.Damage, StatValueType.MinStat, baseStatsData.attackMinValue);
            NetworkedStatServiceLocator.TrySetStatValue<BaseStat>(StatType.Damage, StatValueType.MinScalingStat, baseStatsData.attackMultiplierMinValue);
            
            NetworkedStatServiceLocator.TrySetStatValue<BaseStat>(StatType.Health, StatValueType.Stat, finalHealth);
            NetworkedStatServiceLocator.TrySetStatValue<BaseStat>(StatType.Health, StatValueType.ScalingStat, baseStatsData.healthMultiplierValue);
            NetworkedStatServiceLocator.TrySetStatValue<BaseStat>(StatType.Health, StatValueType.MinStat, baseStatsData.healthMinValue);
            NetworkedStatServiceLocator.TrySetStatValue<BaseStat>(StatType.Health, StatValueType.MinScalingStat, baseStatsData.healthMultiplierMinValue);
            
            NetworkedStatServiceLocator.TrySetStatValue<BaseStat>(StatType.Speed, StatValueType.Stat, baseStatsData.speedValue);
            NetworkedStatServiceLocator.TrySetStatValue<BaseStat>(StatType.Speed, StatValueType.MinStat, baseStatsData.speedMinValue);
            
            NetworkedStatServiceLocator.TrySetStatValue<BaseStat>(StatType.Range, StatValueType.Stat, baseStatsData.rangeValue);
            NetworkedStatServiceLocator.TrySetStatValue<BaseStat>(StatType.Range, StatValueType.MinStat, baseStatsData.rangeMinValue);
            
            NetworkedStatServiceLocator.TrySetStatValue<BaseStat>(StatType.MovementSpeed, StatValueType.Stat, baseStatsData.movementSpeedValue);
            NetworkedStatServiceLocator.TrySetStatValue<BaseStat>(StatType.MovementSpeed, StatValueType.MinStat, baseStatsData.movementSpeedMinValue);
            
            NetworkedStatServiceLocator.TrySetStatValue<BaseStat>(StatType.Stamina, StatValueType.Stat, baseStatsData.staminaValue);
            NetworkedStatServiceLocator.TrySetStatValue<BaseStat>(StatType.Stamina, StatValueType.MinStat, baseStatsData.staminaMinValue);
        }
        
        private void ApplyModToInstantiatedUnit()
        {
            foreach (UnitViewBehaviour unitModBehaviour in unitViewRuntimeSet.GetItems())
            {
                unitModBehaviour.UnitMods.ApplyToInstantiatedUnit(_unitServiceProvider);
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
                NetworkedStatServiceLocator.Register(new LocalModificationStat((StatType)value, scalingStatIdentity, statIdentity));
                NetworkedStatServiceLocator.Register(new BaseStat((StatType)value));

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
                NetworkedStatServiceLocator.Register(new NetworkModificationStat((StatType)statType[i], scalingStatIdentity[i], statIdentity[i]));
            }
        }
    }
}
