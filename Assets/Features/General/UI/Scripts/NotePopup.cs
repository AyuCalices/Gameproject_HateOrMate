using System.Collections;
using ThirdParty.LeanTween.Framework;
using TMPro;
using UnityEngine;

namespace Features.Connection.Scripts
{
    [RequireComponent(typeof(CanvasGroup))]
    public class NotePopup : MonoBehaviour
    {
        [SerializeField] private TMP_Text tmpDescription;
        [SerializeField] private LeanTweenType easeInType;
        [SerializeField] private LeanTweenType easeOutType;
        [SerializeField] private float verticalMovement;
        [SerializeField] private float duration;

        public IEnumerator Instantiate(Transform instantiationParent, string description)
        {
            NotePopup notePopup = Instantiate(this, instantiationParent);
            notePopup.tmpDescription.text = description;
            
            CanvasGroup canvasGroup = notePopup.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            
            RectTransform rectTransform = (RectTransform) notePopup.transform;
            Vector3 localPosition = rectTransform.localPosition;
            Vector3 originPosition = new Vector3(localPosition.x - verticalMovement, localPosition.y);
            Vector3 targetPosition = new Vector3(localPosition.x + verticalMovement, localPosition.y);
            
            rectTransform.localPosition = originPosition;
            LeanTween.move(rectTransform, localPosition, duration / 2).setEase(easeInType).setOnComplete(() =>
            {
                LeanTween.alphaCanvas(canvasGroup, 0, duration / 3).setDelay(duration / 2 - duration / 3).setEase(easeOutType);
                LeanTween.move(rectTransform, targetPosition, duration / 2).setEase(easeOutType).setOnComplete(() => Destroy(notePopup.gameObject));
            });
            LeanTween.alphaCanvas(canvasGroup, 1, duration / 3).setEase(easeInType);

            yield return new WaitForSeconds(duration);
        }
    }
}
