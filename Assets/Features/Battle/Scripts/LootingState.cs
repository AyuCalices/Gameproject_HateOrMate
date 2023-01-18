using System.Collections;
using System.Linq;
using Features.Battle.StateMachine;
using Features.Connection.Scripts;
using Features.Connection.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Features.Battle.Scripts
{
    [CreateAssetMenu]
    public class LootingState : BaseCoroutineState
    {
        public BattleData_SO battleData;
        public  ErrorPopup errorPopup;
        
        private BattleManager _battleManager;
        private Transform _instantiationParent;
        private Button _continueBattleButton;
        private RoomDecisions<bool> _roomDecision;
        private bool _initialized;

        public LootingState Initialize(BattleManager battleManager, Transform instantiationParent, Button continueBattleButton)
        {
            if (_initialized) return this;
            
            _battleManager = battleManager;
            _instantiationParent = instantiationParent;
            _continueBattleButton = continueBattleButton;
            
            _roomDecision = new RoomDecisions<bool>("Placement", false);

            return this;
        }

        public override IEnumerator Enter()
        {
            yield return base.Enter();
            
            _continueBattleButton.interactable = true;
            for (int i = 0; i < _continueBattleButton.transform.childCount; i++)
            {
                _continueBattleButton.transform.GetChild(i).gameObject.SetActive(true);
            }
            
            _continueBattleButton.onClick.AddListener(() =>
            {
                if (battleData.LocalUnitRuntimeSet.GetItems().Any(networkedBattleBehaviour => !networkedBattleBehaviour.IsSpawnedLocally))
                {
                    _roomDecision.SetDecision(true);
                    _continueBattleButton.interactable = false;
                    for (int i = 0; i < _continueBattleButton.transform.childCount; i++)
                    {
                        _continueBattleButton.transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
                else
                {
                    errorPopup.Instantiate(_instantiationParent, "You must at least place one unit into the battle area!");
                }
            });

            Debug.Log("Enter Looting State - Before");
            yield return new WaitForSeconds(2f);
            Debug.Log("Enter Looting State - After");
        }

        public override IEnumerator Exit()
        {
            yield return base.Exit();
            
            Debug.Log("Exit Looting State - Before");
            yield return new WaitForSeconds(2f);
            Debug.Log("Exit Looting State - After");
        }
        
        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            _roomDecision.IsValidDecision(() => _battleManager.RequestStageSetupState(), b => b);
        }
    }
}
