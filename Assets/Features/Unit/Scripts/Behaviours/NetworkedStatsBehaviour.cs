using System;
using Features.Battle.Scripts.StageProgression;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Behaviours.Mod;
using Features.Unit.Scripts.View;
using JetBrains.Annotations;
using Photon.Pun;
using UnityEngine;

namespace Features.Unit.Scripts.Behaviours.Stat
{
    [RequireComponent(typeof(PhotonView))]
    public class NetworkedStatsBehaviour : MonoBehaviourPunCallbacks
    {
        [SerializeField] private ModUnitRuntimeSet_SO modUnitRuntimeSet;
        
        public static Action<NetworkedBattleBehaviour, float, float> onDamageGained;

        public NetworkedStatServiceLocator NetworkedStatServiceLocator { get; private set; }
        public PhotonView PhotonView { get; private set; }

        private float _removedHealth;
        public float RemovedHealth
        {
            get => _removedHealth;
            set
            {
                _removedHealth = value;
                if (TryGetComponent(out UnitBattleView unitView))
                {
                    unitView.SetHealthSlider(value, NetworkedStatServiceLocator.GetTotalValue_CheckMin(StatType.Health));
                }
            }
        }

        public void RaiseDamageGained(NetworkedBattleBehaviour networkedBattleBehaviour, float newRemovedHealth, float totalHealth)
        {
            onDamageGained.Invoke(networkedBattleBehaviour, newRemovedHealth, totalHealth);
        }

        protected void Awake()
        {
            PhotonView = GetComponent<PhotonView>();
            NetworkedStatServiceLocator = new NetworkedStatServiceLocator();
            
            foreach (object value in Enum.GetValues(typeof(StatType)))
            {
                string scalingStatIdentity = Guid.NewGuid().ToString();
                string statIdentity = Guid.NewGuid().ToString();
                NetworkedStatServiceLocator.Register(new LocalStat((StatType)value, scalingStatIdentity, statIdentity));
            }
        }
        
        private void Start()
        {
            ApplyModToInstantiatedUnit();
        }
        
        private void ApplyModToInstantiatedUnit()
        {
            foreach (ModUnitBehaviour unitModBehaviour in modUnitRuntimeSet.GetItems())
            {
                unitModBehaviour.UnitMods.AddModToInstantiatedUnit(this);
            }
        }

        private void OnDestroy()
        {
            NetworkedStatServiceLocator.RemoveAllValues();
        }

        /// <summary>
        /// When instantiating a Unit, call this after a PhotonView ViewId was allocated and the RaiseEvent Instantiation was done. (photon uses a queue)
        /// If the Unit is already placed inside a scene it must be called in Awake
        /// </summary>
        public void OnPhotonViewIdAllocated()
        {
            foreach (object value in Enum.GetValues(typeof(StatType)))
            {
                LocalStat selectedStat = NetworkedStatServiceLocator.Get<LocalStat>((StatType)value);
                PhotonView.RPC("SynchNetworkStat", RpcTarget.Others, selectedStat.StatType, selectedStat.ScalingStatIdentity, selectedStat.StatIdentity);
            }
        }
        
        [PunRPC, UsedImplicitly]
        protected void SynchNetworkStat(StatType statType, string scalingStatIdentity, string statIdentity)
        {
            NetworkedStatServiceLocator.Register(new NetworkStat(statType, scalingStatIdentity, statIdentity));
        }
        
        public void SetBaseStats(BaseStatsGenerator_SO baseStatsGenerator, BaseStatsData_SO baseStatsData)
        {
            baseStatsGenerator.ApplyBaseStats(this, baseStatsData);
        }
    }
}
