using System.Collections;
using Features.Battle.StateMachine;
using Photon.Pun;
using UnityEngine;

namespace Features.Battle.Scripts
{
    [CreateAssetMenu]
    public class EndGameState : BaseCoroutineState
    {
        [SerializeField] private float endScreenTime;
        
        private MusicBehaviour _musicBehaviour;
        private bool _initialized;
    
        private void OnEnable()
        {
            _initialized = false;
        }

        public EndGameState Initialize(MusicBehaviour musicBehaviour)
        {
            if (_initialized) return this;
            
            _musicBehaviour = musicBehaviour;

            return this;
        }

        public override IEnumerator Enter()
        {
            yield return base.Enter();

            if (endScreenTime >= _musicBehaviour.MusicFadeTime)
            {
                yield return new WaitForSeconds(endScreenTime - _musicBehaviour.MusicFadeTime);
            }
            
            _musicBehaviour.Disable();
            yield return new WaitForSeconds(_musicBehaviour.MusicFadeTime);

            PhotonNetwork.Disconnect();
        }
    }
}
