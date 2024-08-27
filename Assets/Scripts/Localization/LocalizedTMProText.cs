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