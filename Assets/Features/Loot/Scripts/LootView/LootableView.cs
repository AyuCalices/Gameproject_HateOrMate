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
        
        public int Identifier { get; private set; }

        private LootableGenerator_SO _lootableGenerator;
        private int _stageAsLevel;

        public void Initialize(LootableGenerator_SO lootableGenerator, int stageAsLevel, int identifier, Action<int> action)
        {
            _lootableGenerator = lootableGenerator;
            _stageAsLevel = stageAsLevel;

            Identifier = identifier;

            Instantiate(lootableGenerator.SpritePrefab, spritePrefabParent);
            description.text = lootableGenerator.Description;
            level.text = stageAsLevel.ToString();
            
            Button button = GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                action.Invoke(identifier);
                button.interactable = false;
                image.color = Color.grey;
            });
        }

        public void GenerateContainedLootable()
        {
            _lootableGenerator.OnAddInstanceToPlayer(_stageAsLevel);
        }
    }
}
