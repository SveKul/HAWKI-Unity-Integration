using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    public TMP_InputField inputField; // TextMeshPro's InputField
    public Button sendButton;
    public TextMeshProUGUI responseText; // TextMeshPro's Text component

    private string chatApiUrl = "https://ki.th-koeln.de/stream-api.php"; // Your chat API URL
    public static List<string> sessionCookies = new List<string>();

    private HttpClient httpClient;

    void Start()
    {
        InitializeHttpClient();
        sendButton.onClick.AddListener(OnSendButtonClicked);
    }

    void InitializeHttpClient()
    {
        var handler = new HttpClientHandler
        {
            UseCookies = false // We will manually handle cookies
        };

        httpClient = new HttpClient(handler);

        foreach (var cookie in sessionCookies)
        {
            httpClient.DefaultRequestHeaders.Add("Cookie", cookie);
        }
    }

    async void OnSendButtonClicked()
    {
        string message = inputField.text;

        if (!string.IsNullOrEmpty(message))
        {
            responseText.text = "Waiting for response...";
            string response = await SendMessageToChatBot(message);
            responseText.text = response;
        }
    }

    async Task<string> SendMessageToChatBot(string message)
    {
        var requestObject = new
        {
            model = "gpt-4o",
            stream = true,
            messages = new List<object> // Ensure this is a list containing objects with `` and `content` properties
            {
                new
                {
                    role = "user", // Correcting the key to ``
                    content = message
                }
            }
        };

        var json = JsonUtility.ToJson(requestObject);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await httpClient.PostAsync(chatApiUrl, content);

        if (response.IsSuccessStatusCode)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            return responseString;
        }
        else
        {
            var errorResponse = await response.Content.ReadAsStringAsync();
            Debug.LogError("Error Response: " + errorResponse); // Log the error response for debugging
            return "Error: " + response.ReasonPhrase;
        }
    }
}