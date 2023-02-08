using System.Globalization;
using Features.Unit.Scripts.Stats;
using TMPro;
using UnityEngine;

namespace Features.UI.Scripts
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
            Debug.Log(text);
            tmpText.text = text;
        }
    }
}
