using TMPro;

public class UIManager
{
    public TMP_InputField InputField { get; set; }
    public TMP_InputField ResponseField { get; set; }

    public void Initialize(TMP_InputField inputField, TMP_InputField responseField)
    {
        InputField = inputField;
        ResponseField = responseField;
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
    }
}