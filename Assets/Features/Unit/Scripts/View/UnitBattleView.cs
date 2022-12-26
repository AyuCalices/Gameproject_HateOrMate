using UnityEngine;

namespace Features.Unit.Scripts.View
{
    //TODO: UniRX
    public class UnitBattleView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform healthTransform;
        [SerializeField] private Transform staminaTransform;

        private void Awake()
        {
            //SetStaminaActive(GetComponent<LocalUnitBehaviour>() != null);
        }

        public void SetHealthActive(bool value)
        {
            healthTransform.gameObject.SetActive(value);
        }

        private void SetStaminaActive(bool value)
        {
            staminaTransform.gameObject.SetActive(value);
        }
    
        public void SetHealthSlider(float removedHealth, float totalHealth)
        {
            float sliderValue = Mathf.Clamp((totalHealth - removedHealth) / totalHealth, 0, 1);
            Vector3 localScale = healthTransform.localScale;
            Vector3 newScale = new Vector3(sliderValue, localScale.y, localScale.z);
            healthTransform.localScale = newScale;
        }

        public void SetStaminaSlider(float currentStamina, float totalStamina)
        {
            if (totalStamina == 0) return;
            
            float sliderValue = Mathf.Clamp(currentStamina / totalStamina, 0, 1);
            Vector3 localScale = staminaTransform.localScale;
            Vector3 newScale = new Vector3(sliderValue, localScale.y, localScale.z);
            staminaTransform.localScale = newScale;
        }

        public void ResetStaminaSlider()
        {
            Vector3 localScale = staminaTransform.localScale;
            Vector3 newScale = new Vector3(1, localScale.y, localScale.z);
            staminaTransform.localScale = newScale;
        }
    }
}
