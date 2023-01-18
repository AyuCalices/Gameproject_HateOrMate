using System.Globalization;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Behaviours.Stat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Loot.UI.CharacterSelect
{
    public class UnitModViewBehaviour : MonoBehaviour
    {
        [SerializeField] private Image unitSprite;
        [SerializeField] private TMP_Text unitName;
        [SerializeField] private TMP_Text damageText;
        [SerializeField] private TMP_Text healthText;
        [SerializeField] private TMP_Text speedText;
        [SerializeField] private TMP_Text movementText;
    
        private NetworkedStatsBehaviour _unitOwnerStats;
        private BattleBehaviour _unitOwnerBattleBehaviour;

        public void Initialize(NetworkedStatsBehaviour unitOwnerStats)
        {
            _unitOwnerStats = unitOwnerStats;
            _unitOwnerBattleBehaviour = unitOwnerStats.GetComponent<BattleBehaviour>();
        
        }
    
        private void Update()
        {
            //TODO: remove from update
            if (_unitOwnerStats == null) return;

            if (_unitOwnerBattleBehaviour.UnitClassData != null)
            {
                if (_unitOwnerBattleBehaviour.UnitClassData.sprite != null)
                {
                    unitSprite.sprite = _unitOwnerBattleBehaviour.UnitClassData.sprite;
                }
                unitName.text = _unitOwnerBattleBehaviour.UnitClassData.unitType.unitName;
            }
        
            damageText.text = _unitOwnerStats.NetworkedStatServiceLocator.Get<LocalStat>(StatType.Damage).GetTotalValue().ToString(CultureInfo.CurrentCulture);
            healthText.text = _unitOwnerStats.NetworkedStatServiceLocator.Get<LocalStat>(StatType.Health).GetTotalValue().ToString(CultureInfo.CurrentCulture);
            speedText.text = _unitOwnerStats.NetworkedStatServiceLocator.Get<LocalStat>(StatType.Speed).GetTotalValue().ToString(CultureInfo.CurrentCulture);
            movementText.text = _unitOwnerBattleBehaviour.UnitClassData.movementSpeed.ToString(CultureInfo.CurrentCulture);

        }
    }
}
