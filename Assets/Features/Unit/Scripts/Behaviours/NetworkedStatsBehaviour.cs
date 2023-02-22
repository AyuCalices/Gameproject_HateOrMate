using System;
using System.Globalization;
using DataStructures.Event;
using Features.Battle.Scripts;
using Features.Battle.StateMachine;
using Features.Loot.Scripts;
using Features.Loot.Scripts.ModView;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Behaviours.Mod;
using Features.Unit.Scripts.Stats;
using Features.Unit.Scripts.View;
using JetBrains.Annotations;
using Photon.Pun;
using UnityEngine;

namespace Features.Unit.Scripts.Behaviours
{
    [RequireComponent(typeof(PhotonView))]
    public class NetworkedStatsBehaviour : MonoBehaviourPunCallbacks
    {
        [SerializeField] private BattleData_SO battleData;
        [SerializeField] private UnitViewRuntimeSet_SO unitViewRuntimeSet;
        [SerializeField] private CanvasFocus_SO canvasFocus;
        [SerializeField] private DamagePopup damagePopupPrefab;
        [SerializeField] private GameEvent onHit;
        
        public static Action<NetworkedBattleBehaviour, float, float> onDamageGained;

        public NetworkedStatServiceLocator NetworkedStatServiceLocator { get; private set; }
        public PhotonView PhotonView { get; private set; }

        private float _removedHealth;
        public int Level { get; private set; }
        public float RemovedHealth
        {
            get => _removedHealth;
            set
            {
                if (battleData.StateIsValid(typeof(BattleState), StateProgressType.Execute))
                {
                    damagePopupPrefab.Create(
                        canvasFocus.Get().transform, 
                        Mathf.FloorToInt(value - _removedHealth).ToString(CultureInfo.CurrentCulture), 
                        Color.yellow, 
                        20, 
                        transform.position);
                    
                    onHit.Raise();
                }

                _removedHealth = value;
                if (TryGetComponent(out UnitBattleView unitView))
                {
                    unitView.SetHealthSlider(value, GetFinalStat(StatType.Health));
                }
            }
        }

        public void RaiseDamageGained(NetworkedBattleBehaviour networkedBattleBehaviour, float newRemovedHealth, float totalHealth)
        {
            onDamageGained?.Invoke(networkedBattleBehaviour, newRemovedHealth, totalHealth);
        }

        protected void Awake()
        {
            PhotonView = GetComponent<PhotonView>();
            NetworkedStatServiceLocator = new NetworkedStatServiceLocator();
            
            foreach (object value in Enum.GetValues(typeof(StatType)))
            {
                string scalingStatIdentity = Guid.NewGuid().ToString();
                string statIdentity = Guid.NewGuid().ToString();
                NetworkedStatServiceLocator.Register(new LocalModificationStat((StatType)value, scalingStatIdentity, statIdentity));
                NetworkedStatServiceLocator.Register(new BaseStat((StatType)value));
            }
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
                unitModBehaviour.UnitMods.ApplyToInstantiatedUnit(this);
            }
        }

        private void OnDestroy()
        {
            NetworkedStatServiceLocator.UnregisterAll();
        }

        /// <summary>
        /// When instantiating a Unit, call this after a PhotonView ViewId was allocated and the RaiseEvent Instantiation was done. (photon uses a queue)
        /// If the Unit is already placed inside a scene it must be called in Awake
        /// </summary>
        public void OnPhotonViewIdAllocated()
        {
            foreach (object value in Enum.GetValues(typeof(StatType)))
            {
                LocalModificationStat selectedModificationStat = NetworkedStatServiceLocator.Get<LocalModificationStat>((StatType)value);
                PhotonView.RPC("SynchNetworkStat", RpcTarget.Others, selectedModificationStat.StatType, selectedModificationStat.MultiplierStatIdentity, selectedModificationStat.StatIdentity);
            }
        }
        
        [PunRPC, UsedImplicitly]
        protected void SynchNetworkStat(StatType statType, string scalingStatIdentity, string statIdentity)
        {
            NetworkedStatServiceLocator.Register(new NetworkModificationStat(statType, scalingStatIdentity, statIdentity));
        }
        
        public void SetBaseStats(BaseStatsData_SO baseStatsData, int level)
        {
            Level = level;
            
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
