using System;
using Features.Unit.Stat;
using JetBrains.Annotations;
using Photon.Pun;
using UnityEditor;
using UnityEngine;

namespace Features.Unit
{
    public class NetworkedUnitBehaviour : MonoBehaviour
    {
        [SerializeField] private NetworkedUnitRuntimeSet_SO networkedPlayerUnits;
    
        private PhotonView _photonView;

        public NetworkedUnitRuntimeSet_SO NetworkedPlayerUnits => networkedPlayerUnits;
    
        public NetworkedStatServiceLocator NetworkedStatServiceLocator { get; private set; }
        public int ViewID { get; private set; }
        public bool NetworkingInitialized { get; private set; }

        protected void Awake()
        {
            _photonView = GetComponent<PhotonView>();
        
            NetworkedStatServiceLocator = new NetworkedStatServiceLocator();

            InternalAwake();
        }
        
        protected virtual void InternalAwake() {}

        protected void Start()
        {
            //Networking only possible post Awake with the used Architecture: The UnitSpawner Instatiates the Unit and gives it a ViewID. Applying the ViewID happens after the Awake of this Script and gets synchronized between all Clients, resulting that networking must be used in Start(). This has the reason, that we want to differentiate between a Networked Unit and a Local Unit (e.g. for instantiating the ModUI only for the Local Unit)
            ViewID = _photonView.ViewID;
            foreach (object value in Enum.GetValues(typeof(StatType)))
            {
                string scalingStatIdentity = GUID.Generate().ToString();
                string statIdentity = GUID.Generate().ToString();
                NetworkedStatServiceLocator.Register(new LocalStat((StatType)value, scalingStatIdentity, statIdentity));
                _photonView.RPC("SynchNetworkStat", RpcTarget.Others, (StatType)value, scalingStatIdentity, statIdentity);
            }
            NetworkingInitialized = true;

            AddToRuntimeSet();
            InternalStart();
        }
        
        protected virtual void InternalStart() {}
    
        [PunRPC, UsedImplicitly]
        protected void SynchNetworkStat(StatType statType, string scalingStatIdentity, string statIdentity)
        {
            NetworkedStatServiceLocator.Register(new NetworkStat(statType, scalingStatIdentity, statIdentity));
        }

        private void OnDestroy()
        {
            RemoveFromRuntimeSet();
        }

        protected void Update()
        {
            //Debug.Log(NetworkedStatServiceLocator.GetTotalValue(StatType.Damage));
        }

        protected virtual void AddToRuntimeSet()
        {
            networkedPlayerUnits.Add(this);
        }
    
        protected virtual void RemoveFromRuntimeSet()
        {
            networkedPlayerUnits.Remove(this);
        }
    }
}
