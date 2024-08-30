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

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager
{
    public TMP_InputField InputField { get; set; }
    public TMP_InputField ResponseField { get; set; }
    public ScrollRect ScrollRect { get; set; } // Hinzugefügt: ScrollRect-Referenz

    public void Initialize(TMP_InputField inputField, TMP_InputField responseField, ScrollRect scrollRect)
    {
        InputField = inputField;
        ResponseField = responseField;
        ScrollRect = scrollRect; // ScrollRect-Zuweisung
        ResponseField.readOnly = true;
    }

    public void AddMessageToResponse(string message)
    {
        ResponseField.text += message;
    }

    public void AddUserMessage(string message)
    {
        AddMessageToResponse($"\nUser: {message}\n\n");
    }

    public void AddWaitingMessage()
    {
        AddMessageToResponse(LocalizationManager.GetLocalizedText(TextKey.WaitingForResponse));
    }

    public void RemoveWaitingMessage()
    {
        string waitingText = LocalizationManager.GetLocalizedText(TextKey.WaitingForResponse);
        if (ResponseField.text.EndsWith(waitingText))
        {
            ResponseField.text = ResponseField.text.Replace(waitingText, "");
        }
    }

    public void ResetUI()
    {
        InputField.text = string.Empty;
        InputField.placeholder.GetComponent<TextMeshProUGUI>().text = LocalizationManager.GetLocalizedText(TextKey.AssistInitialResponse);
        ResponseField.text = string.Empty;
        
        // Caret und Text-Position zurücksetzen
        ResetInputFieldTransform(InputField);
        ResetInputFieldTransform(ResponseField);
    }

    private void ResetInputFieldTransform(TMP_InputField inputField)
    {
        // Caret RectTransform zurücksetzen
        RectTransform caretTransform = inputField.textViewport.GetChild(0).GetComponent<RectTransform>();
        caretTransform.offsetMin = new Vector2(caretTransform.offsetMin.x, 0); // Bottom
        caretTransform.offsetMax = new Vector2(caretTransform.offsetMax.x, 0); // Top

        // Text RectTransform zurücksetzen
        RectTransform textTransform = inputField.textComponent.GetComponent<RectTransform>();
        textTransform.offsetMin = new Vector2(textTransform.offsetMin.x, 0); // Bottom
        textTransform.offsetMax = new Vector2(textTransform.offsetMax.x, 0); // Top
        
        // Layout-Update erzwingen
        LayoutRebuilder.ForceRebuildLayoutImmediate(inputField.GetComponent<RectTransform>());
    }
    
}