using Features.Unit.Stat;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Features.Unit
{
    public class UnitBehaviour : MonoBehaviour
    {
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

            UnitMods = new UnitMods(modCount, this);
            NetworkedStatServiceLocator = new NetworkedStatServiceLocator(_photonView);
            
            AddToRuntimeSet();
        }
        
        private void OnDestroy()
        {
            RemoveFromRuntimeSet();
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
