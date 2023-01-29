using System.Globalization;
using Features.Unit.Scripts.Behaviours.Stat;
using TMPro;
using UnityEngine;

namespace Features.UI.Scripts
{
    public class StatTextUpdateBehaviour : MonoBehaviour
    {
        [SerializeField] private TMP_Text tmpText;
        [SerializeField] private StatType statType;

        public StatType StatType => statType;
        
        private float _modifierValue;
        private float _scaleValue = 1f;

        public void UpdateText(float modifierValueAdditive, float scaleValueAdditive, float baseValue = 0)
        {
            _modifierValue += modifierValueAdditive;
            _scaleValue += scaleValueAdditive;
            tmpText.text = (baseValue + _modifierValue * _scaleValue).ToString(CultureInfo.CurrentCulture);
        }
    }
}
