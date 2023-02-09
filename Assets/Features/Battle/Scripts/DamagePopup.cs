using TMPro;
using UnityEngine;

namespace Features.Battle.Scripts
{
    public class DamagePopup : MonoBehaviour
    {
        private const float DisappearTimerMax = 1f;

        private TextMeshProUGUI _textMesh;
        private float _disappearTimer;
        private Color _textColor;
        private Vector3 _moveVector;

        private void Awake()
        {
            _textMesh = transform.GetComponent<TextMeshProUGUI>();
        }
        
        public void Create(Transform parent, string popupMessage, Color popupColor, int fontSize, Vector3 position)
        {
            GameObject damagePopupTransform = Instantiate(gameObject, parent);
            damagePopupTransform.transform.position = position;

            DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
            damagePopup.Setup(popupMessage, popupColor, fontSize);
        }

        private void Setup(string popupMessage, Color popupColor, int fontSize)
        {
            _textMesh.SetText(popupMessage);
            
            _textMesh.color = popupColor;
            _textMesh.fontSize = fontSize;
            _disappearTimer = 1f;

            _moveVector = new Vector3(0.13f, 0.2f) * 30f;
        }

        private void Update()
        {
            transform.position += _moveVector * Time.deltaTime;
            _moveVector -= _moveVector * 5f * Time.deltaTime;

            if (_disappearTimer > DisappearTimerMax * 0.5f)
            {
                float increaseScaleAmount = 1f;
                transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
            }
            else
            {
                float decreaseScaleAmount = 1f;
                transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
            }

            _disappearTimer -= Time.deltaTime;
            if (_disappearTimer < 0)
            {
                float disappearSpeed = 3f;
                _textColor.a -= disappearSpeed * Time.deltaTime;
                _textMesh.color = _textColor;

                if (_textColor.a < 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
