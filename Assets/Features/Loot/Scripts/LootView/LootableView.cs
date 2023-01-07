using System;
using Features.Loot.Scripts.Generator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Loot.Scripts.LootView
{
    [RequireComponent(typeof(Button))]
    public class LootableView : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private Transform spritePrefabParent;
        [SerializeField] private TMP_Text description;
        [SerializeField] private TMP_Text level;

        private LootableGenerator_SO _lootableGenerator;
        private int _stageAsLevel;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        public void Initialize(LootableGenerator_SO lootableGenerator, int stageAsLevel, Func<bool> action)
        {
            _lootableGenerator = lootableGenerator;
            _stageAsLevel = stageAsLevel;

            Instantiate(lootableGenerator.SpritePrefab, spritePrefabParent);
            description.text = lootableGenerator.Description;
            level.text = stageAsLevel.ToString();
            
            _button.onClick.AddListener(() =>
            {
                if (action.Invoke())
                {
                    _button.interactable = false;
                    image.color = Color.grey;
                }
            });
        }

        public void GenerateContainedLootable()
        {
            _lootableGenerator.OnAddInstanceToPlayer(_stageAsLevel);
        }
    }
}
