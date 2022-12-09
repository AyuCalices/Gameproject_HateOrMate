using System;
using Features.Battle;
using Features.Battle.Scripts;
using Features.GlobalReferences;
using Features.GlobalReferences.Scripts;
using Features.Unit.Battle;
using Features.Unit.Battle.Scripts;
using Features.Unit.Modding.Stat;
using Features.Unit.View;
using JetBrains.Annotations;
using Photon.Pun;
using UnityEngine;

namespace Features.Unit.Modding
{
    public class NetworkedUnitBehaviour : MonoBehaviour
    {
        [SerializeField] protected BattleData_SO battleData;
        public NetworkedUnitRuntimeSet_SO OwnerNetworkedPlayerUnits => ownerNetworkedPlayerUnits;
        [SerializeField] private NetworkedUnitRuntimeSet_SO ownerNetworkedPlayerUnits;
        
        
        public NetworkedUnitRuntimeSet_SO EnemyRuntimeSet { get; protected set; }
        public UnitControlType ControlType { get; protected set; }
        
        
        public NetworkedStatServiceLocator NetworkedStatServiceLocator { get; private set; }
        public PhotonView PhotonView { get; private set; }
        public bool NetworkingInitialized { get; private set; }

        private float _removedHealth;
        public float RemovedHealth
        {
            get => _removedHealth;
            set
            {
                _removedHealth = value;
                if (TryGetComponent(out UnitView unitView))
                {
                    unitView.SetHealthSlider(RemovedHealth, NetworkedStatServiceLocator.GetTotalValue(StatType.Health));
                }
            }
        }

        public void StageCheck()
        {
            battleData.BattleManager.StageCheck();
        }

        protected void Awake()
        {
            PhotonView = GetComponent<PhotonView>();
            NetworkedStatServiceLocator = new NetworkedStatServiceLocator();
            
            InternalAwake();
        }

        /// <summary>
        /// When instantiating a Unit, call this after a PhotonView ViewId was allocated and the RaiseEvent Instantiation was done. (photon uses a queue)
        /// If the Unit is already placed inside a scene it must be called in Awake
        /// </summary>
        public void OnPhotonViewIdAllocated()
        {
            Debug.Log("Init Networking");
            
            foreach (object value in Enum.GetValues(typeof(StatType)))
            {
                string scalingStatIdentity = Guid.NewGuid().ToString();
                string statIdentity = Guid.NewGuid().ToString();
                NetworkedStatServiceLocator.Register(new LocalStat((StatType)value, scalingStatIdentity, statIdentity));
                PhotonView.RPC("SynchNetworkStat", RpcTarget.Others, (StatType)value, scalingStatIdentity, statIdentity);
            }

            NetworkingInitialized = true;
            InternalOnNetworkingEnabled();

            if (TryGetComponent(out BattleBehaviour battleBehaviour))
            {
                battleBehaviour.OnNetworkingEnabled();
            }
        }
        
        protected virtual void InternalOnNetworkingEnabled()
        { 
            NetworkedStatServiceLocator.SetBaseValue(StatType.Damage, 10);
            NetworkedStatServiceLocator.SetBaseValue(StatType.Health, 50);
            NetworkedStatServiceLocator.SetBaseValue(StatType.Speed, 3);
        }
        
        [PunRPC, UsedImplicitly]
        protected void SynchNetworkStat(StatType statType, string scalingStatIdentity, string statIdentity)
        {
            NetworkedStatServiceLocator.Register(new NetworkStat(statType, scalingStatIdentity, statIdentity));
        }

        protected virtual void InternalAwake()
        {
            EnemyRuntimeSet = battleData.EnemyUnitRuntimeSet;
            
            ownerNetworkedPlayerUnits.Add(this);
            battleData.PlayerTeamUnitRuntimeSet.Add(this);
            
            if (PhotonNetwork.IsMasterClient)
            {
                ControlType = UnitControlType.Master;
            }
            else
            {
                ControlType = UnitControlType.Client;
            }
        }

        protected void Start()
        {
            InternalStart();
        }
        
        protected virtual void InternalStart() { }

        private void Update()
        {
            //Debug.Log(gameObject.name + " " + NetworkedStatServiceLocator.GetTotalValue(StatType.Damage));
        }

        private void OnDestroy()
        {
            InternalOnDestroy();
        }

        protected virtual void InternalOnDestroy()
        {
            ownerNetworkedPlayerUnits.Remove(this);
            battleData.PlayerTeamUnitRuntimeSet.Remove(this);
        }
    }
}
