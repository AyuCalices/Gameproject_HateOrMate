using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Connection.Scripts
{
    public class ErrorPopup : MonoBehaviour
    {
        [SerializeField] private TMP_Text tmpDescription;
        [SerializeField] private Button button;

        public void Instantiate(Transform instantiationParent, string description, Action onButtonClick = null)
        {
            ErrorPopup errorPopup = Instantiate(this, instantiationParent);
            errorPopup.tmpDescription.text = description;
            errorPopup.button.onClick.AddListener(() =>
            {
                onButtonClick?.Invoke();
                Destroy(errorPopup.gameObject);
            });
        }
    }
}
