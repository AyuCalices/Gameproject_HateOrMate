using System.Globalization;
using Features.Unit.Scripts.Behaviours.Services.UnitStats;
using TMPro;
using UnityEngine;

namespace Features.Mods.Scripts.View
{
    public class StatTextUpdateBehaviour : MonoBehaviour
    {
        [SerializeField] private TMP_Text tmpText;
        [SerializeField] private StatType statType;

        public StatType StatType => statType;
        
        private float _scaleValue;

        public void UpdateText(float percent)
        {
            _scaleValue += percent;
            tmpText.text = (_scaleValue * 100).ToString(CultureInfo.CurrentCulture) + " %";
        }

        public void UpdateText(string text)
        {
            tmpText.text = text;
        }
    }
}
