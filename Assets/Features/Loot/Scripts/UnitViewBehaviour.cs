using System;
using System.Collections.Generic;
using System.Linq;
using Features.Loot.Scripts.GeneratedLoot;
using Features.UI.Scripts;
using Features.Unit.Scripts;
using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Loot.Scripts
{
    public class UnitViewBehaviour : MonoBehaviour
    {
        [SerializeField] private UnitType_SO towerUnitType;
        [SerializeField] private Image unitSprite;
        [SerializeField] protected List<StatTextUpdateBehaviour> statTextUpdateBehaviours;
        [SerializeField] protected TMP_Text nameText;
    
        private NetworkedStatsBehaviour _unitOwnerStats;
        private BattleBehaviour _unitOwnerBattleBehaviour;

        private void OnDestroy()
        {
            _unitOwnerStats.NetworkedStatServiceLocator.onUpdateStat -= UpdateSingleStatText;
        }
        
        private void UpdateSingleStatText(StatType statType)
        {
            foreach (StatTextUpdateBehaviour statTextUpdateBehaviour in statTextUpdateBehaviours
                .Where(statTextUpdateBehaviour => statTextUpdateBehaviour.StatType == statType))
            {
                statTextUpdateBehaviour.UpdateText(_unitOwnerStats.GetFinalStat(statType).ToString());
                return;
            }
        }

        public void Initialize(NetworkedStatsBehaviour unitOwnerStats)
        {
            _unitOwnerStats = unitOwnerStats;
            _unitOwnerBattleBehaviour = unitOwnerStats.GetComponent<BattleBehaviour>();
            
            _unitOwnerStats.NetworkedStatServiceLocator.onUpdateStat += UpdateSingleStatText;
            
            InitializeVisualization();
            InitializeAllText();
        }

        private void InitializeVisualization()
        {
            unitSprite.sprite = _unitOwnerBattleBehaviour.UnitClassData.sprite;
            nameText.text = _unitOwnerBattleBehaviour.UnitClassData.unitType.unitName;
        }
        
        private void InitializeAllText()
        {
            UnitType_SO unitType = _unitOwnerBattleBehaviour.UnitClassData.unitType;
            StatTextUpdateBehaviour staminaStatTextUpdateBehaviours = statTextUpdateBehaviours.Find(x => x.StatType == StatType.Stamina);
            staminaStatTextUpdateBehaviours.gameObject.SetActive(towerUnitType == unitType);
        }
    }
}
