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
        

        protected void Awake()
        {
            _unitServiceProvider = GetComponent<UnitServiceProvider>();
            NetworkedStatServiceLocator = new NetworkedStatServiceLocator();
            
            foreach (object value in Enum.GetValues(typeof(StatType)))
            {
                string scalingStatIdentity = Guid.NewGuid().ToString();
                string statIdentity = Guid.NewGuid().ToString();
                NetworkedStatServiceLocator.Register(new LocalModificationStat((StatType)value, scalingStatIdentity, statIdentity));
                NetworkedStatServiceLocator.Register(new BaseStat((StatType)value));
            }

            if (_unitServiceProvider.GetService<PhotonView>().ViewID != 0)
            {
                foreach (object value in Enum.GetValues(typeof(StatType)))
                {
                    LocalModificationStat selectedModificationStat = NetworkedStatServiceLocator.Get<LocalModificationStat>((StatType)value);
                    _unitServiceProvider.GetService<PhotonView>().RPC("SynchNetworkStat", RpcTarget.Others, selectedModificationStat.StatType, selectedModificationStat.MultiplierStatIdentity, selectedModificationStat.StatIdentity);
                }
            }
            else
            {
                Debug.Log("View ID Not Allocated");
            }
        }
        
        //TODO: reduce RPC count
        [PunRPC, UsedImplicitly]
        protected void SynchNetworkStat(StatType statType, string scalingStatIdentity, string statIdentity)
        {
            NetworkedStatServiceLocator.Register(new NetworkModificationStat(statType, scalingStatIdentity, statIdentity));
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
        
        private void Start()
        {
            ApplyModToInstantiatedUnit();
        }
        
        private void ApplyModToInstantiatedUnit()
        {
            foreach (UnitViewBehaviour unitModBehaviour in unitViewRuntimeSet.GetItems())
            {
                unitModBehaviour.UnitMods.ApplyToInstantiatedUnit(_unitServiceProvider);
            }
        }

        private void OnDestroy()
        {
            NetworkedStatServiceLocator.UnregisterAll();
        }

        public void SetBaseStats(BaseStatsData_SO baseStatsData, int level)
        {
            float finalAttack = baseStatsData.attackBaseValue * Mathf.Pow(baseStatsData.attackLevelScaling, level);
            float finalHealth = baseStatsData.healthBaseValue * Mathf.Pow(baseStatsData.healthLevelScaling, level);

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
    }
}
