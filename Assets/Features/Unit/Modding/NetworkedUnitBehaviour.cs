using System;
using ExitGames.Client.Photon;
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
using Photon.Realtime;
using UnityEngine;

namespace Features.Unit.Modding
{
    [RequireComponent(typeof(PhotonView))]
    public class NetworkedUnitBehaviour : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] protected BattleData_SO battleData;
        
        [Header("Team Selection")]
        [SerializeField] protected NetworkedUnitRuntimeSet_SO ownerNetworkedPlayerUnits;
        [SerializeField] protected NetworkedUnitRuntimeSet_SO enemyRuntimeSet;
        [SerializeField] protected NetworkedUnitRuntimeSet_SO ownTeamRuntimeSet;

        public NetworkedUnitRuntimeSet_SO EnemyRuntimeSet => enemyRuntimeSet;

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
                //TODO: getComponent
                if (TryGetComponent(out UnitView unitView))
                {
                    unitView.SetHealthSlider(RemovedHealth, NetworkedStatServiceLocator.GetTotalValue(StatType.Health));
                }
            }
        }

        protected void Awake()
        {
            //TODO: getComponent
            PhotonView = GetComponent<PhotonView>();
            NetworkedStatServiceLocator = new NetworkedStatServiceLocator();
            battleData.AllUnitsRuntimeSet.Add(this);
            
            InternalAwake();
        }
        
        protected virtual void InternalAwake()
        {
            if (ownerNetworkedPlayerUnits != null)
            {
                ownerNetworkedPlayerUnits.Add(this);
            }
            ownTeamRuntimeSet.Add(this);
        }

        /// <summary>
        /// When instantiating a Unit, call this after a PhotonView ViewId was allocated and the RaiseEvent Instantiation was done. (photon uses a queue)
        /// If the Unit is already placed inside a scene it must be called in Awake
        /// </summary>
        public void OnPhotonViewIdAllocated()
        {
            foreach (object value in Enum.GetValues(typeof(StatType)))
            {
                string scalingStatIdentity = Guid.NewGuid().ToString();
                string statIdentity = Guid.NewGuid().ToString();
                NetworkedStatServiceLocator.Register(new LocalStat((StatType)value, scalingStatIdentity, statIdentity));
                PhotonView.RPC("SynchNetworkStat", RpcTarget.Others, (StatType)value, scalingStatIdentity, statIdentity);
            }

            NetworkingInitialized = true;
            InternalOnNetworkingEnabled();

            //TODO: getComponent
            if (TryGetComponent(out NetworkedBattleBehaviour battleBehaviour))
            {
                battleBehaviour.OnNetworkingEnabled();
            }
        }
        
        [PunRPC, UsedImplicitly]
        protected void SynchNetworkStat(StatType statType, string scalingStatIdentity, string statIdentity)
        {
            NetworkedStatServiceLocator.Register(new NetworkStat(statType, scalingStatIdentity, statIdentity));
        }
        
        protected virtual void InternalOnNetworkingEnabled()
        { 
            NetworkedStatServiceLocator.SetBaseValue(StatType.Damage, 10);
            NetworkedStatServiceLocator.SetBaseValue(StatType.Health, 50);
            NetworkedStatServiceLocator.SetBaseValue(StatType.Speed, 3);
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
            
            battleData.AllUnitsRuntimeSet.Remove(this);
        }

        protected virtual void InternalOnDestroy()
        {
            if (ownerNetworkedPlayerUnits != null)
            {
                ownerNetworkedPlayerUnits.Remove(this);
            }

            ownTeamRuntimeSet.Remove(this);
        }
    }
}
