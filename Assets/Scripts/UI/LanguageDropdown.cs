using UnityEngine;
using TMPro;

[ExecuteAlways]
public class LanguageDropdown : MonoBehaviour
{
    private TMP_Dropdown languageDropdown;

    void Awake()
    {
        // Directly reference the connected TMP_Dropdown component
        languageDropdown = GetComponent<TMP_Dropdown>();

        // Ensure the dropdown component exists
        if (languageDropdown == null)
        {
            Debug.LogError("TMP_Dropdown component is missing.");
            return;
        }

        // Add listener for when the Dropdown value changes
        languageDropdown.onValueChanged.AddListener(DropdownValueChanged);

        // Set dropdown to current language on awake
        SetDropdownToCurrentLanguage();
    }

    void SetDropdownToCurrentLanguage()
    {
        if (languageDropdown == null) return;

        languageDropdown.options.Clear();
        languageDropdown.options.Add(new TMP_Dropdown.OptionData("English"));
        languageDropdown.options.Add(new TMP_Dropdown.OptionData("German"));

        // Set the value based on the current language
        if (LocalizationManager.CurrentLanguage == "German")
        {
            languageDropdown.value = 1;
        }
        else
        {
            languageDropdown.value = 0;
        }
    }

    void DropdownValueChanged(int change)
    {
        switch (change)
        {
            case 0:
                LocalizationManager.CurrentLanguage = "English";
                break;
            case 1:
                LocalizationManager.CurrentLanguage = "German";
                break;
            default:
                LocalizationManager.CurrentLanguage = "English";
                break;
        }
    }

    void OnValidate()
    {
        // Validate and update dropdown options in the editor
        if (languageDropdown == null)
        {
            languageDropdown = GetComponent<TMP_Dropdown>();
        }
        
        SetDropdownToCurrentLanguage();
    }

    void OnDestroy()
    {
        // Remove listener to prevent memory leaks
        if (languageDropdown != null)
        {
            languageDropdown.onValueChanged.RemoveListener(DropdownValueChanged);
        }
    }
}