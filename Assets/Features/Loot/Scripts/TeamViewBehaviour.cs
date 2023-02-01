using System;
using System.Collections.Generic;
using System.Linq;
using Features.Loot.Scripts.GeneratedLoot;
using Features.UI.Scripts;
using Features.Unit.Scripts.Behaviours.Stat;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace Features.Loot.Scripts
{
    public class TeamViewBehaviour : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TeamTagType teamTagType;
        [SerializeField] protected List<StatTextUpdateBehaviour> statTextUpdateBehaviours;
        [SerializeField] protected TMP_Text nameText;

        //TODO: create class, that contains multiple stats on which units grab modifiers when instantiating/adding
        protected virtual void UpdateStatText(StatTextUpdateBehaviour statTextUpdateBehaviour, StatType statType, 
            float modifierValue, float scaleValue) => statTextUpdateBehaviour.UpdateText(modifierValue, scaleValue);

        protected virtual void Awake()
        {
            MultipleStatMod.onRegisterGlobally += UpdateMultipleStatText;
            MultipleStatMod.onUnregisterGlobally += UpdateMultipleStatText;
        }

        protected virtual void OnDestroy()
        {
            MultipleStatMod.onRegisterGlobally -= UpdateMultipleStatText;
            MultipleStatMod.onUnregisterGlobally -= UpdateMultipleStatText;
        }
        
        private void UpdateMultipleStatText(TeamTagType[] teamTagTypes, StatType statType, float modifierValue, float scaleValue)
        {
            if (!teamTagTypes.Contains(teamTagType)) return;

            foreach (StatTextUpdateBehaviour statTextUpdateBehaviour in statTextUpdateBehaviours)
            {
                if (statTextUpdateBehaviour.StatType == statType)
                {
                    UpdateStatText(statTextUpdateBehaviour, statType, modifierValue, scaleValue);
                }
            }
        }
        
        public void Initialize()
        {
            InitializeVisualization();
            UpdateMultipleStatText(new TeamTagType[] {teamTagType}, StatType.Damage, 0, 0);
            UpdateMultipleStatText(new TeamTagType[] {teamTagType}, StatType.Health, 0, 0);
            UpdateMultipleStatText(new TeamTagType[] {teamTagType}, StatType.Range, 0, 0);
            UpdateMultipleStatText(new TeamTagType[] {teamTagType}, StatType.Speed, 0, 0);
            UpdateMultipleStatText(new TeamTagType[] {teamTagType}, StatType.Stamina, 0, 0);
            UpdateMultipleStatText(new TeamTagType[] {teamTagType}, StatType.MovementSpeed, 0, 0);
        }

        protected virtual void InitializeVisualization()
        {
            nameText.text = teamTagType.ToString();
        }
    }
}
