using DataStructures.StateLogic;
using ExitGames.Client.Photon;
using Features.Battle;
using Features.Unit.Modding;
using Features.Unit.Modding.Stat;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Features.Unit.Battle
{
    [RequireComponent(typeof(NetworkedUnitBehaviour), typeof(PhotonView), typeof(UnitView))]
    public class BattleBehaviour : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        [SerializeField] private BattleManager_SO battleManager;
        [SerializeField] private float range;
        
        private NetworkedUnitBehaviour _networkedUnitBehaviour;
        private StateMachine _stateMachine;
        private BattleActions _battleActions;
        private PhotonView _photonView;
        private UnitView _unitView;

        public BattleActions BattleActions => _battleActions;

        //1st step: send damage + animation behaviour from attacker to calculating instance - Client & Master: Send to others
        //2nd step: raise event to update health on all clients on attacked instance
        //AI events gets called by masterClient: If Client Attack on AI -> Send Attack To Master
        private float _removedHealth;

        public float RemovedHealth
        {
            get => _removedHealth;
            set
            {
                _removedHealth = value;
                _unitView.SetHealthSlider(_removedHealth, _networkedUnitBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Health));
            }
        }

        private void Awake()
        {
            _stateMachine = new StateMachine();
            _stateMachine.Initialize(new IdleState());
            _networkedUnitBehaviour = GetComponent<NetworkedUnitBehaviour>();
            
            _photonView = GetComponent<PhotonView>();
            _unitView = GetComponent<UnitView>();

            _removedHealth = 0;
            _battleActions = new TowerBattleActions(_networkedUnitBehaviour, battleManager.EnemyUnitRuntimeSet, _unitView,10, 10);
            //_battleActions = new TroopBattleActions(_networkedUnitBehaviour, battleManager.EnemyUnitRuntimeSet);
            
            EnterAttackState();
        }

        private void Update()
        {
            /*
            switch (_stateMachine.CurrentState)
            {
                case MovementState when battleManager.EnemyUnitRuntimeSet.IsInRangeByWorldPosition(range, transform.position):
                    EnterAttackState();
                    break;
                case AttackState when !battleManager.EnemyUnitRuntimeSet.IsInRangeByWorldPosition(range, transform.position):
                    EnterMovementState();
                    break;
            }*/

            _stateMachine.Update();
        }
        
        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == RaiseEventCode.OnPerformUnitAttack)
            {
                Debug.Log(photonEvent.Code);
                object[] data = (object[]) photonEvent.CustomData;
                if (photonView.ViewID == (int) data[0])
                {
                    _battleActions.OnSendAttackActionCallback(this, (float) data[1]);
                }
            }
            else if (photonEvent.Code == RaiseEventCode.OnPerformUpdateUnitHealth)
            {
                Debug.Log(photonEvent.Code);
                object[] data = (object[]) photonEvent.CustomData;
                if (photonView.ViewID == (int) data[0])
                {
                    _battleActions.OnSendHealthActionCallback(this, (float) data[1], _networkedUnitBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Health));
                }
            }
        }

        public void EnterIdleState()
        {
            _stateMachine.ChangeState(new IdleState());
        }

        public void EnterAttackState()
        {
            _stateMachine.ChangeState(new AttackState(_battleActions));
        }

        public void EnterMovementState()
        {
            _stateMachine.ChangeState(new MovementState(_battleActions));
        }
    }
}
