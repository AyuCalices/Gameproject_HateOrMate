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

        public LootableGenerator_SO LootableGenerator { get; private set; }

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        public void Initialize(LootableGenerator_SO lootableGenerator, Func<bool> action)
        {
            LootableGenerator = lootableGenerator;

            Instantiate(lootableGenerator.SpritePrefab, spritePrefabParent);
            description.text = lootableGenerator.Description;
            
            _button.onClick.AddListener(() =>
            {
                if (action.Invoke())
                {
                    _button.interactable = false;
                    image.color = Color.grey;
                }
            });
        }
    }
}
