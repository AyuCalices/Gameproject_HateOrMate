using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextUpdateBehaviour : MonoBehaviour
{
    [SerializeField] private TMP_Text tmpText;

    public void UpdateText(string text)
    {
        tmpText.text = text;
    }
}
