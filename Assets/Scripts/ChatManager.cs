using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button sendButton;
    public TextMeshProUGUI responseText;

    private string _chatApiUrl;
    public static List<string> sessionCookies = new List<string>();

    private HttpClient _httpClient;
    private bool _isReceivingData = false;
    private string _domain;
    private string _model;

    // List to store all chat messages
    private List<object> _chatMessages = new List<object>();

    void Start()
    {
        ConfigLoader configLoader = new ConfigLoader();
        _domain = configLoader.LoadDomainFromConfig();
        _model = configLoader.LoadModelFromConfig();
        _chatApiUrl = $"{_domain}/stream-api.php";
        
        InitializeHttpClient();
        sendButton.onClick.AddListener(OnSendButtonClicked);
    }

    void InitializeHttpClient()
    {
        var handler = new HttpClientHandler
        {
            UseCookies = false // Manually handle cookies
        };

        _httpClient = new HttpClient(handler);

        foreach (var cookie in sessionCookies)
        {
            _httpClient.DefaultRequestHeaders.Add("Cookie", cookie);
        }
    }

    async void OnSendButtonClicked()
    {
        string message = inputField.text;

        if (!string.IsNullOrEmpty(message))
        {
            responseText.text = "Waiting for response...";
            await SendMessageToChatBot(message);
        }
    }

    async Task SendMessageToChatBot(string message)
    {
        // Add the new user message to the list.
        _chatMessages.Add(new { role = "user", content = message });

        var requestObject = new
        {
            model = _model,
            stream = true,
            messages = _chatMessages
        };

        string json = JsonConvert.SerializeObject(requestObject);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _httpClient.PostAsync(_chatApiUrl, content);

        if (response.IsSuccessStatusCode)
        {
            var responseStream = await response.Content.ReadAsStreamAsync();
            await ProcessResponseStream(responseStream);
        }
        else
        {
            var errorResponse = await response.Content.ReadAsStringAsync();
            Debug.LogError("Error Response: " + errorResponse);
            responseText.text = "Error: " + response.ReasonPhrase;
        }
    }

    async Task ProcessResponseStream(System.IO.Stream responseStream)
    {
        using (var reader = new System.IO.StreamReader(responseStream))
        {
            string line;
            StringBuilder responseContent = new StringBuilder();

            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (line.StartsWith("data: "))
                {
                    line = line.Substring(6).Trim(); // Remove "data: " part

                    // Check if line is "[]" or empty
                    if (line == "[]" || line == "[]")
                    {
                        break;
                    }

                    // Handle JSON array case
                    if (line.StartsWith("["))
                    {
                        continue; // Skip arrays if there are any
                    }

                    try
                    {
                        var chunk = JsonConvert.DeserializeObject<ResponseChunk>(line);
                        if (chunk != null && chunk.choices.Count > 0 && chunk.choices[0].delta != null)
                        {
                            if (chunk.choices[0].delta.content != null)
                            {
                                responseContent.Append(chunk.choices[0].delta.content);
                                responseText.text = responseContent.ToString();
                            }
                        }
                    }
                    catch (JsonSerializationException e)
                    {
                        Debug.LogError("JSON Deserialization error: " + e.Message);
                    }
                }
            }

            // Adding AI response to chatMessages list
            //chatMessages.Add(new { = "assistant", content = responseContent.ToString() });
        }
    }

    private class ResponseChunk
    {
        public string id { get; set; }
        public string @object { get; set; }
        public int created { get; set; }
        public string model { get; set; }
        public string system_fingerprint { get; set; }
        public List<Choice> choices { get; set; }

        public class Choice
        {
            public int index { get; set; }
            public Delta delta { get; set; }
            public object logprobs { get; set; }
            public string finish_reason { get; set; }
        }

        public class Delta
        {
            //public string { get; set; }
            public string content { get; set; }
        }
    }
}