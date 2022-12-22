using System;
using Features.Unit.Modding.Stat;
using Features.Unit.View;
using JetBrains.Annotations;
using Photon.Pun;
using UnityEngine;

namespace Features.Unit.Modding
{
    [RequireComponent(typeof(PhotonView))]
    public class NetworkedStatsBehaviour : MonoBehaviour
    {
        public NetworkedStatServiceLocator NetworkedStatServiceLocator { get; private set; }
        public PhotonView PhotonView { get; private set; }

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
            PhotonView = GetComponent<PhotonView>();
            NetworkedStatServiceLocator = new NetworkedStatServiceLocator();
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

            NetworkedStatServiceLocator.SetBaseValue(StatType.Damage, 10);
            NetworkedStatServiceLocator.SetBaseValue(StatType.Health, 50);
            NetworkedStatServiceLocator.SetBaseValue(StatType.Speed, 3);
        }
        
        [PunRPC, UsedImplicitly]
        protected void SynchNetworkStat(StatType statType, string scalingStatIdentity, string statIdentity)
        {
            NetworkedStatServiceLocator.Register(new NetworkStat(statType, scalingStatIdentity, statIdentity));
        }
    }
}
