using System.Collections.Generic;
using System.Linq;
using Features.Mods.Scripts.ModTypes;
using Features.Unit.Scripts.Behaviours.Services.BattleBehaviour;
using Features.Unit.Scripts.Behaviours.Services.UnitStats;
using TMPro;
using UnityEngine;

namespace Features.Mods.Scripts.View
{
    //TODO: create class, that contains multiple stats on which units grab modifiers when instantiating/adding
    public class TeamDisplayBehaviour : MonoBehaviour
    {
        [SerializeField] private TeamTagType teamTagType;
        [SerializeField] protected List<StatTextUpdateBehaviour> statTextUpdateBehaviours;
        [SerializeField] protected TMP_Text nameText;

        private void Awake()
        {
            MultipleStatMod.onRegisterGlobally += UpdateMultipleStatText;
            MultipleStatMod.onUnregisterGlobally += UpdateMultipleStatText;

            Initialize();
        }

        private void OnDestroy()
        {
            MultipleStatMod.onRegisterGlobally -= UpdateMultipleStatText;
            MultipleStatMod.onUnregisterGlobally -= UpdateMultipleStatText;
        }
        
        private void UpdateMultipleStatText(TeamTagType[] teamTagTypes, StatType statType, float percent)
        {
            if (!teamTagTypes.Contains(teamTagType)) return;

            foreach (StatTextUpdateBehaviour statTextUpdateBehaviour in statTextUpdateBehaviours)
            {
                if (statTextUpdateBehaviour.StatType == statType)
                {
                    statTextUpdateBehaviour.UpdateText(percent);
                }
            }
        }
        
        private void Initialize()
        {
            InitializeVisualization();
            UpdateMultipleStatText(new TeamTagType[] {teamTagType}, StatType.Damage, 0);
            UpdateMultipleStatText(new TeamTagType[] {teamTagType}, StatType.Health, 0);
            UpdateMultipleStatText(new TeamTagType[] {teamTagType}, StatType.Range, 0);
            UpdateMultipleStatText(new TeamTagType[] {teamTagType}, StatType.Speed, 0);
            UpdateMultipleStatText(new TeamTagType[] {teamTagType}, StatType.Stamina, 0);
            UpdateMultipleStatText(new TeamTagType[] {teamTagType}, StatType.MovementSpeed, 0);
        }

        private void InitializeVisualization()
        {
            nameText.text = teamTagType.ToString();
        }
    }
}