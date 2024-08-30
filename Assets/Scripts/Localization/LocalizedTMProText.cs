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
public class LocalizedTMProText : MonoBehaviour, ILocalizationObserver
{
    public TextKey textKey;  // Enum reference to the text key
    private TextMeshProUGUI textMeshPro;

    void Awake()
    {
        // Get the TextMeshProUGUI component on this GameObject
        textMeshPro = GetComponent<TextMeshProUGUI>();

        if (textMeshPro == null)
        {
            Debug.LogError("TextMeshProUGUI component is missing on this GameObject.");
            return;
        }
        
        // Register with LocalizationManager
        LocalizationManager.Subscribe(this);

        // Set the initial text based on the current language
        UpdateText();
    }

    public void OnLanguageChanged()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        // Update the TextMeshPro text with the localized text
        if (textMeshPro != null)
        {
            textMeshPro.text = LocalizationManager.GetLocalizedText(textKey);
        }
    }

    void OnDestroy()
    {
        // Unregister from the LocalizationManager
        LocalizationManager.Unsubscribe(this);
    }

    void OnValidate()
    {
        // Update text in the editor when the TextKey is changed
        if (textMeshPro == null)
        {
            textMeshPro = GetComponent<TextMeshProUGUI>();
        }

        UpdateText();
    }

    void Update()
    {
        // Update text if we're running in the editor
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UpdateText();
        }
#endif
    }
}