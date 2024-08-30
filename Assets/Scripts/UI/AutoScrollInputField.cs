// --------------------------------------------------------------------------------------------------------------------
// Copyright (C) 2023 TH Köln – University of Applied Sciences

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.
// --------------------------------------------------------------------------------------------------------------------

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