using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitView : MonoBehaviour
{
    [SerializeField] private Transform healthTransform;
    [SerializeField] private Transform staminaAndSpeedTransform;

    public void SetHealthSlider(float removedHealth, float totalHealth)
    {
        Debug.Log(removedHealth + " " + totalHealth);
        float sliderValue = Mathf.Clamp((totalHealth - removedHealth) / totalHealth, 0, 1);
        Vector3 localScale = healthTransform.localScale;
        Vector3 newScale = new Vector3(sliderValue, localScale.y, localScale.z);
        healthTransform.localScale = newScale;
    }

    public void SetStaminaSlider(float currentStamina, float totalStamina)
    {
        float sliderValue = Mathf.Clamp(currentStamina / totalStamina, 0, 1);
        Vector3 localScale = staminaAndSpeedTransform.localScale;
        Vector3 newScale = new Vector3(sliderValue, localScale.y, localScale.z);
        staminaAndSpeedTransform.localScale = newScale;
    }

    public void SetSpeedSlider(float currentSpeed, float totalSpeed)
    {
        float sliderValue = Mathf.Clamp(currentSpeed / totalSpeed, 0, 1);
        Vector3 localScale = staminaAndSpeedTransform.localScale;
        Vector3 newScale = new Vector3(sliderValue, localScale.y, localScale.z);
        staminaAndSpeedTransform.localScale = newScale;
    }
}
