using TMPro;
using UnityEngine;

[ExecuteAlways]
public class LocalizedInputField : MonoBehaviour
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

        // Set the initial placeholder text based on the current language
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
