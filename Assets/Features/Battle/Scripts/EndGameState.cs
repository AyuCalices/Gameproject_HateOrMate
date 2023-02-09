using System.Collections;
using DataStructures.Event;
using DataStructures.ReactiveVariable;
using Features.Battle.StateMachine;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Features.Battle.Scripts
{
    [CreateAssetMenu]
    public class EndGameState : BaseCoroutineState
    {
        [SerializeField] private float endScreenTime;
        
        private BattleManager _battleManager;
        private bool _initialized;
    
        private void OnEnable()
        {
            _initialized = false;
        }

        public EndGameState Initialize(BattleManager battleManager)
        {
            if (_initialized) return this;
            
            _battleManager = battleManager;

            return this;
        }

        public override IEnumerator Enter()
        {
            yield return base.Enter();

            yield return new WaitForSeconds(endScreenTime);
            
            PhotonNetwork.Disconnect();
        }
    }
}
