using Features.Unit.Modding;
using Features.Unit.Modding.Stat;
using UnityEngine;

namespace Features.Unit.Battle
{
    [RequireComponent(typeof(NetworkedUnitBehaviour))]
    public class BattleBehaviour : MonoBehaviour
    {
        [SerializeField] private BattleActions_SO battleActions;

        private float _attackDeltaTime;
        private NetworkedUnitBehaviour _networkedUnitBehaviour;
    
        //TODO: implement State Machine for Movement, Attack & Idle

        private void Awake()
        {
            _networkedUnitBehaviour = GetComponent<NetworkedUnitBehaviour>();
        }

        private void Update()
        {
            //TODO: implement cast speed. When attack -> target, caster inside Attack stat
            //TODO: implement moving to enemy, if no enemy in reach -> if enemy in reach swap to attackState
            //TODO: send event for attackTime UI updated
        
            _attackDeltaTime -= Time.deltaTime;

            if (_attackDeltaTime > 0) return;

            //Attack(unit);
            _attackDeltaTime = _networkedUnitBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Speed);
        }
    }
}
