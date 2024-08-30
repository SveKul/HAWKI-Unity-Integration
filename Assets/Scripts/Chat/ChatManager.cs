using System.Collections;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
public class ChatManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button sendButton;
    public Button clearSessionButton;
    public TMP_InputField responseInputField;

    private string _chatApiUrl;
    public static List<string> sessionCookies = new List<string>();

    private HttpClientManager _httpClientManager;
    private ConfigManager _configManager;
    private UIManager _uiManager;
    private ChatSessionManager _chatSessionManager;

    void Start()
    {
        _configManager = new ConfigManager();

        _chatApiUrl = $"{_configManager.Domain}/stream-api.php";
        _httpClientManager = new HttpClientManager(sessionCookies, _chatApiUrl);

        _uiManager = new UIManager();
        _uiManager.Initialize(inputField, responseInputField);

        _chatSessionManager = new ChatSessionManager(_uiManager);

        sendButton.onClick.AddListener(OnSendButtonClicked);
        clearSessionButton.onClick.AddListener(_chatSessionManager.ClearSession);
    }

    void OnSendButtonClicked()
    {
        string message = inputField.text;

        if (!string.IsNullOrEmpty(message))
        {
            _uiManager.AddUserMessage(message);
            _chatSessionManager.AddUserMessage(message);
            StartCoroutine(SendMessageToChatBot(message));

            inputField.text = string.Empty;
            inputField.placeholder.GetComponent<TextMeshProUGUI>().text = LocalizationManager.GetLocalizedText(TextKey.RequestInitialPlaceholder);
        }
    }

    IEnumerator SendMessageToChatBot(string message)
    {
        _uiManager.AddWaitingMessage();

        var requestObject = new
        {
            model = _configManager.Model,
            stream = true,
            messages = _chatSessionManager.GetChatMessages()
        };

        string json = JsonConvert.SerializeObject(requestObject);
        var responseTask = _httpClientManager.PostAsync(json, _chatApiUrl);

        yield return new WaitUntil(() => responseTask.IsCompleted);

        if (responseTask.Result.IsSuccessStatusCode)
        {
            var streamTask = _httpClientManager.GetResponseStreamAsync(responseTask.Result);
            yield return new WaitUntil(() => streamTask.IsCompleted);

            StartCoroutine(ProcessResponseStream(streamTask.Result));
            _uiManager.RemoveWaitingMessage();
        }
        else
        {
            var errorResponseTask = responseTask.Result.Content.ReadAsStringAsync();
            yield return new WaitUntil(() => errorResponseTask.IsCompleted);

            Debug.LogError("Error Response: " + errorResponseTask.Result);
            _uiManager.AddMessageToResponse($"\nError: {responseTask.Result.ReasonPhrase}\n");
        }
    }

    IEnumerator ProcessResponseStream(Stream responseStream)
    {
        StringBuilder responseContent = new StringBuilder();

        using (var reader = new StreamReader(responseStream))
        {
            while (!reader.EndOfStream)
            {
                Task<string> readLineTask = reader.ReadLineAsync();
                yield return new WaitUntil(() => readLineTask.IsCompleted);

                string line = readLineTask.Result;
                if (line == null)
                {
                    break;
                }

                if (line.StartsWith("data: "))
                {
                    line = line.Substring(6).Trim(); // Remove "data: " part

                    if (line == "[]" || string.IsNullOrEmpty(line))
                    {
                        break;
                    }

                    if (line.StartsWith("["))
                    {
                        continue;
                    }

                    try
                    {
                        var chunk = JsonConvert.DeserializeObject<ResponseChunk>(line);
                        if (chunk != null && chunk.choices.Count > 0 && chunk.choices[0].delta != null)
                        {
                            if (chunk.choices[0].delta.content != null)
                            {
                                responseContent.Append(chunk.choices[0].delta.content);
                                _uiManager.AddMessageToResponse(chunk.choices[0].delta.content);
                            }
                        }
                    }
                    catch (JsonSerializationException e)
                    {
                        Debug.LogError("JSON Deserialization error: " + e.Message);
                    }
                }

                yield return null;
            }

            _chatSessionManager.AddAssistantMessage(responseContent.ToString());

            _uiManager.AddMessageToResponse("\n\n");
        }
    }
}