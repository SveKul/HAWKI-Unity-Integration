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

[ExecuteAlways]
public class LocalizedInputField : MonoBehaviour, ILocalizationObserver
{
    public TextKey placeholderKey;  // Enum reference to the text key for the placeholder
    private TMP_InputField inputField;
    private TextMeshProUGUI placeholderText;

    void Awake()
    {
        // Get the TMP_InputField component on this GameObject
        inputField = GetComponent<TMP_InputField>();

        if (inputField == null)
        {
            Debug.LogError("TMP_InputField component is missing on this GameObject.");
            return;
        }

        placeholderText = inputField.placeholder as TextMeshProUGUI;

        if (placeholderText == null)
        {
            Debug.LogError("Placeholder is not assigned or is not TextMeshProUGUI component.");
            return;
        }

        // Register with LocalizationManager
        LocalizationManager.Subscribe(this);

        // Set the initial placeholder text based on the current language
        UpdatePlaceholderText();
    }

    public void OnLanguageChanged()
    {
        UpdatePlaceholderText();
    }

    public void UpdatePlaceholderText()
    {
        // Update the TMP_InputField placeholder text with the localized text
        if (placeholderText != null)
        {
            placeholderText.text = LocalizationManager.GetLocalizedText(placeholderKey);
        }
    }

    void OnDestroy()
    {
        // Unregister from the LocalizationManager
        LocalizationManager.Unsubscribe(this);
    }

    void OnValidate()
    {
        // Update placeholder text in the editor when the TextKey is changed
        if (inputField == null)
        {
            inputField = GetComponent<TMP_InputField>();
        }

        if (placeholderText == null)
        {
            placeholderText = inputField.placeholder as TextMeshProUGUI;
        }

        UpdatePlaceholderText();
    }

    void Update()
    {
        // Update placeholder text if we're running in the editor
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UpdatePlaceholderText();
        }
#endif
    }
}
