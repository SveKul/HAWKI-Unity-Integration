using TMPro;
using UnityEngine;

public class LoginDebugLogger
{
    private TextMeshProUGUI _debugText;

    public LoginDebugLogger(TextMeshProUGUI debugText)
    {
        _debugText = debugText;
    }

    public void Log(string message)
    {
        if (_debugText != null)
        {
            _debugText.text = message;
        }
        else
        {
            Debug.LogWarning("Debug TextMeshProUGUI is not assigned.");
        }
    }
}