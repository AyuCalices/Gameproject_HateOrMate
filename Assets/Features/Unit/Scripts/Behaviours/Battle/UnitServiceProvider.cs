using System;
using UnityEngine;
using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.View;
using Photon.Pun;

namespace Features.Battle.Scripts.Unit.ServiceLocatorSystem
{
    public class UnitServiceProvider : MonoBehaviour
    {
        private readonly ServiceLocatorObject<MonoBehaviour> _unitServiceController = new ServiceLocatorObject<MonoBehaviour>();
        private bool _isInitialized;

        private NetworkedStatsBehaviour _networkedStatsBehaviour;
        private NetworkedBattleBehaviour _networkedBattleBehaviour;
        private UnitDragPlacementBehaviour _unitDragPlacementBehaviour;
        private UnitBattleView _unitBattleView;
        private PhotonView _photonView;

        private void Awake()
        {
            _networkedStatsBehaviour = GetComponent<NetworkedStatsBehaviour>();
            _networkedBattleBehaviour = GetComponent<NetworkedBattleBehaviour>();
            _unitDragPlacementBehaviour = GetComponent<UnitDragPlacementBehaviour>();
            _unitBattleView = GetComponent<UnitBattleView>();
            _photonView = GetComponent<PhotonView>();

            Initialize();
        }

        public void Initialize()
        {
            _isInitialized = true;
            _unitServiceController.Register(this);
            _unitServiceController.Register(_networkedStatsBehaviour);
            _unitServiceController.Register(_networkedBattleBehaviour);
            _unitServiceController.Register(_unitDragPlacementBehaviour);
            _unitServiceController.Register(_unitBattleView);
            _unitServiceController.Register(_photonView);
        }

        public T GetService<T>() where T : MonoBehaviour
        {
            if (!_isInitialized)
            {
                Debug.LogError("The Service Provider has not been initialized yet!");
            }
            
            return _unitServiceController.Get<T>();
        }

        public bool TryGetService<T>(out T service) where T : MonoBehaviour
        {
            if (!_isInitialized)
            {
                Debug.LogError("The Service Provider has not been initialized yet!");
            }

            if (_unitServiceController.TryGetService(out T requestedService))
            {
                service = requestedService;
                return true;
            }

            service = default;
            return false;
        }
    }
}
