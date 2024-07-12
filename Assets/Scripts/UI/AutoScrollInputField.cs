using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AutoScrollInputField : MonoBehaviour
{
    public ScrollRect scrollRect;
    public TMP_InputField inputField;

    private void Start()
    {
        // Event hinzufügen, das aufgerufen wird, wenn sich der Text ändert
        inputField.onValueChanged.AddListener(UpdateScrollPosition);
    }

    private void UpdateScrollPosition(string text)
    {
        // Warteframe um Sicherzustellen, dass TMP-Layout sich aktualisiert
        StartCoroutine(AdjustScrollPosition());
    }

    private IEnumerator AdjustScrollPosition()
    {
        // Einmalige Verzögerung um dem Layout Zeit zum Aktualisieren zu geben
        yield return null;

        // Vertikale Höhe des Textes berechnen
        float scrollHeight = inputField.textViewport.rect.height;
        float textHeight = inputField.textComponent.preferredHeight;

        if (textHeight > scrollHeight)
        {
            // Positiere den Scrollbar auf dem unteren Ende des Textes
            float extraScroll = textHeight - scrollHeight;
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01((1 - extraScroll / scrollHeight));
        }
        else
        {
            scrollRect.verticalNormalizedPosition = 1; // Kein Scrollen nötig
        }
    }
}