using DataStructures.ReactiveVariable;
using TMPro;
using UnityEngine;

namespace Features.BattleScene.Scripts.States
{
    public class GameEndBehaviour : MonoBehaviour
    {
        [SerializeField] private IntReactiveVariable stageReactiveVariable;
        [SerializeField] private IntReactiveVariable completedStageReactiveVariable;
        [SerializeField] private GameObject objectToHide;
        [SerializeField] private TMP_Text resultText;

        private void OnEnable()
        {
            if (stageReactiveVariable.Get() >= completedStageReactiveVariable.Get())
            {
                objectToHide.SetActive(true);
                resultText.text = "Game Completed!";
            }
            else
            {
                objectToHide.SetActive(false);
                resultText.text = "Game Failed!";
            }
        }
    }
}