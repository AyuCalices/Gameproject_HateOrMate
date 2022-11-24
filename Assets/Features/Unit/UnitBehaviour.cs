using System;
using System.Collections.Generic;
using Features.Unit.Stat;
using JetBrains.Annotations;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor;
using UnityEngine;

namespace Features.Unit
{
    public class UnitBehaviour : MonoBehaviour
    {
        [SerializeField] private Canvas unitModHUD;
        
        [SerializeField] private UnitRuntimeSet_SO localPlayerUnits;
        [SerializeField] private UnitRuntimeSet_SO externPlayerUnits;
        [SerializeField] private int modCount;
        
        private bool _isOwner;
        private PhotonView _photonView;

        public UnitRuntimeSet_SO LocalPlayerUnits => localPlayerUnits;
        public UnitRuntimeSet_SO ExternPlayerUnits => externPlayerUnits;
        
        public NetworkedStatServiceLocator NetworkedStatServiceLocator { get; private set; }
        public UnitMods UnitMods { get; private set; }
        public string Identity { get; private set; }
        

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            Identity = (string) _photonView.InstantiationData[0];
            _isOwner = Equals((Player)_photonView.InstantiationData[1], PhotonNetwork.LocalPlayer);

            List<ModSlotBehaviour> modDropBehaviours = Instantiate(unitModHUD).GetComponentInChildren<UnitModHud>().GetAllChildren();
            UnitMods = new UnitMods(modCount, this, modDropBehaviours);
            NetworkedStatServiceLocator = new NetworkedStatServiceLocator();
            foreach (object value in Enum.GetValues(typeof(StatType)))
            {
                string scalingStatIdentity = GUID.Generate().ToString();
                string statIdentity = GUID.Generate().ToString();
                NetworkedStatServiceLocator.Register(new LocalStat((StatType)value, scalingStatIdentity, statIdentity));
                _photonView.RPC("SynchNetworkStat", RpcTarget.All, (StatType)value, scalingStatIdentity, statIdentity, PhotonNetwork.LocalPlayer);
            }

            AddToRuntimeSet();
        }
        
        [PunRPC, UsedImplicitly]
        private void SynchNetworkStat(StatType statType, string scalingStatIdentity, string statIdentity, Player sender)
        {
            if (Equals(sender, PhotonNetwork.LocalPlayer)) return;
            NetworkedStatServiceLocator.Register(new NetworkStat(statType, scalingStatIdentity, statIdentity));
        }

        private void OnDestroy()
        {
            RemoveFromRuntimeSet();
        }

        private void Update()
        {
            Debug.Log(NetworkedStatServiceLocator.GetTotalValue(StatType.Damage));
        }

        private void AddToRuntimeSet()
        {
            if (_isOwner)
            {
                localPlayerUnits.Add(this);
            }
            else
            {
                externPlayerUnits.Add(this);
            }
        }
        
        private void RemoveFromRuntimeSet()
        {
            if (_isOwner)
            {
                localPlayerUnits.Remove(this);
            }
            else
            {
                externPlayerUnits.Remove(this);
            }
        }
    }
}
