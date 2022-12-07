using System;
using Features.Mod;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Loot
{
    [RequireComponent(typeof(Button))]
    public class LootableView : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TMP_Text lootableName;
        [SerializeField] private TMP_Text description;

        public LootableGenerator_SO LootableGenerator { get; private set; }

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }


        public void Initialize(LootableGenerator_SO lootableGenerator, Action action)
        {
            LootableGenerator = lootableGenerator;
            
            lootableName.text = lootableGenerator.ModName;
            description.text = lootableGenerator.Description;
            
            _button.onClick.AddListener(() =>
            {
                _button.interactable = false;
                image.color = Color.grey;
                action.Invoke();
            });
        }
    }
}
