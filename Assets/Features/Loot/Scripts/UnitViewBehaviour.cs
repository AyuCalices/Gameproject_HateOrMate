using Features.Loot.Scripts.GeneratedLoot;
using Features.UI.Scripts;
using Features.Unit.Scripts;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Behaviours.Stat;
using Features.Unit.Scripts.Stats;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Loot.Scripts
{
    public class UnitViewBehaviour : TeamViewBehaviour
    {
        [SerializeField] private UnitType_SO towerUnitType;
        [SerializeField] private Image unitSprite;
    
        private NetworkedStatsBehaviour _unitOwnerStats;
        private BattleBehaviour _unitOwnerBattleBehaviour;

        protected override void UpdateStatText(StatTextUpdateBehaviour statTextUpdateBehaviour, StatType statType, float modifierValue, float scaleValue) => 
            statTextUpdateBehaviour.UpdateText(modifierValue, scaleValue, _unitOwnerStats.NetworkedStatServiceLocator.Get<BaseStat>(statType).GetTotalValue());

        protected override void Awake()
        {
            base.Awake();
            
            SingleStatMod.onRegister += UpdateSingleStatText;
            SingleStatMod.onUnregister += UpdateSingleStatText;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            SingleStatMod.onRegister -= UpdateSingleStatText;
            SingleStatMod.onUnregister -= UpdateSingleStatText;
        }
        
        private void UpdateSingleStatText(NetworkedStatsBehaviour networkedStatsBehaviour, StatType statType, float modifierValue, float scaleValue)
        {
            if (_unitOwnerStats != networkedStatsBehaviour) return;

            foreach (StatTextUpdateBehaviour statTextUpdateBehaviour in statTextUpdateBehaviours)
            {
                if (statTextUpdateBehaviour.StatType == statType)
                {
                    UpdateStatText(statTextUpdateBehaviour, statType, modifierValue, scaleValue);
                }
            }
        }
        
        public void Initialize(NetworkedStatsBehaviour unitOwnerStats)
        {
            _unitOwnerStats = unitOwnerStats;
            _unitOwnerBattleBehaviour = unitOwnerStats.GetComponent<BattleBehaviour>();
            
            InitializeVisualization();
            InitializeAllText();
        }
        
        protected override void InitializeVisualization()
        {
            unitSprite.sprite = _unitOwnerBattleBehaviour.UnitClassData.sprite;
            nameText.text = _unitOwnerBattleBehaviour.UnitClassData.unitType.unitName;
        }
        
        private void InitializeAllText()
        {
            UnitType_SO unitType = _unitOwnerBattleBehaviour.UnitClassData.unitType;

            UpdateSingleStatText(_unitOwnerStats, StatType.Damage, 0, 0);
            UpdateSingleStatText(_unitOwnerStats, StatType.Health, 0, 0);
            UpdateSingleStatText(_unitOwnerStats, StatType.Range, 0, 0);
            UpdateSingleStatText(_unitOwnerStats, StatType.MovementSpeed, 0, 0);

            StatTextUpdateBehaviour staminaStatTextUpdateBehaviours = statTextUpdateBehaviours.Find(x => x.StatType == StatType.Stamina);
            if (towerUnitType == unitType)
            {
                staminaStatTextUpdateBehaviours.gameObject.SetActive(true);
                UpdateSingleStatText(_unitOwnerStats, StatType.Stamina, 0, 0);
            }
            else
            {
                staminaStatTextUpdateBehaviours.gameObject.SetActive(false);
            }
            
            UpdateSingleStatText(_unitOwnerStats, StatType.Speed, 0, 0);
        }
    }
}
