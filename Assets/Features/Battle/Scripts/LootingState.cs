using System.Collections;
using ExitGames.Client.Photon;
using Features.Battle.StateMachine;
using Features.Connection.Scripts.Utils;
using Features.Loot.Scripts.Generator;
using UnityEngine;

namespace Features.Battle.Scripts
{
    [CreateAssetMenu]
    public class LootingState : BaseCoroutineState
    {
        [SerializeField] private BattleData_SO battleData;
        
        private BattleManager _battleManager;
        private bool _initialized;
        
        private void OnEnable()
        {
            _initialized = false;
        }

        public LootingState Initialize(BattleManager battleManager)
        {
            if (_initialized) return this;
            
            _battleManager = battleManager;

            return this;
        }

        public override IEnumerator Enter()
        {
            yield return base.Enter();
            
            Debug.Log("Enter Looting State - Before");
            yield return new WaitForSeconds(2f);
            Debug.Log("Enter Looting State - After");

            if (battleData.LootCount == 0)
            {
                _battleManager.RequestPlacementState();
            }
        }

        public override IEnumerator Exit()
        {
            yield return base.Exit();
            
            Debug.Log("Exit Looting State - Before");
            yield return new WaitForSeconds(2f);
            Debug.Log("Exit Looting State - After");
        }
    }
}
