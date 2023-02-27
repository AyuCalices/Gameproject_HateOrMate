using System.Linq;
using Features.Battle.Scripts;
using Features.Connection.Scripts;
using Features.Connection.Scripts.Utils;
using Features.Unit.Scripts.Behaviours.Battle;
using Plugins.UniRx.Extensions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Features.UI.Scripts
{
    public class GameViewBehaviour : MonoBehaviour
    {
        [SerializeField] private BattleData_SO battleData;
        [SerializeField] private TextMeshProUGUI stageText;
        [SerializeField] private Button continueBattleButton;
        [SerializeField] private Button requestLootPhaseButton;
        [SerializeField] public ErrorPopup errorPopup;
        [SerializeField] private BoolRoomDecitions_SO continueButtonRoomDecision;
        [SerializeField] private BoolRoomDecitions_SO requestLootPhaseButtonRoomDecision;
    
        private void Start()
        {
            SetRequestLootPhaseButtonInteractable(false);
            AddLootPhaseButtonListener();
        
            AddContinueBattleButtonListener();

            RegisterStageText();
        }
    
        private void AddLootPhaseButtonListener()
        {
            requestLootPhaseButton.onClick.AddListener(() =>
            {
                requestLootPhaseButtonRoomDecision.SetDecision(true);
                SetRequestLootPhaseButtonInteractable(false);
            });
        }

        private void AddContinueBattleButtonListener()
        {
            continueBattleButton.onClick.AddListener(() =>
            {
                if (battleData.AllUnitsRuntimeSet.GetUnitsByTag(TeamTagType.Own).Any(
                    unitServiceProvider => unitServiceProvider.GetService<NetworkedBattleBehaviour>().CurrentState is not BenchedState))
                {
                    continueButtonRoomDecision.SetDecision(true);
                    SetContinueButtonInteractable(false);
                }
                else
                {
                    errorPopup.Instantiate(transform.root, "You must at least place one unit into the battle area!");
                }
            });
        }

        private void RegisterStageText()
        {
            battleData.Stage.RuntimeProperty
                .Select(x => x + "/" + battleData.CompletedStage.Get())
                .SubscribeToText(stageText);
        }
    
        public void SetRequestLootPhaseButtonInteractable(bool value)
        {
            requestLootPhaseButton.interactable = value;
        }

        public void SetContinueButtonInteractable(bool value)
        {
            continueBattleButton.interactable = value;
            for (int i = 0; i < continueBattleButton.transform.childCount; i++)
            {
                continueBattleButton.transform.GetChild(i).gameObject.SetActive(value);
            }
        }
    }
}
