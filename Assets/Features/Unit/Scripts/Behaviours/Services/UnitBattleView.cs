using System.Globalization;
using Features.Battle.Scripts;
using Features.Loot.Scripts.ModView;
using UnityEngine;

namespace Features.Unit.Scripts.View
{
    public class UnitBattleView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CanvasFocus_SO canvasFocus;
        [SerializeField] private DamagePopup damagePopupPrefab;
        [SerializeField] protected SpriteRenderer unitSprite;
        [SerializeField] private Transform healthTransform;
        [SerializeField] private Transform staminaTransform;

        public void Initialize(Sprite sprite, bool setHealthActive, bool setStaminaActive)
        {
            unitSprite.sprite = sprite;

            SetHealthActive(setHealthActive);
            SetStaminaActive(setStaminaActive);
        }

        private void SetHealthActive(bool value)
        {
            healthTransform.gameObject.SetActive(value);
        }

        private void SetStaminaActive(bool value)
        {
            staminaTransform.gameObject.SetActive(value);
        }
        
        public void InstantiateDamagePopup(float value) 
        {
            damagePopupPrefab.Create(
                canvasFocus.Get().transform, 
                Mathf.FloorToInt(value).ToString(CultureInfo.CurrentCulture), 
                Color.yellow, 
                20, 
                transform.position);
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
