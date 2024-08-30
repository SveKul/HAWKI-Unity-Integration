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

using System.Collections;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using System.Collections.Generic;

public class ChatManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button sendButton;
    public Button clearSessionButton;
    public TMP_InputField responseInputField;
    public ScrollRect ScrollRect;

    private string _chatApiUrl;
    public static List<string> sessionCookies = new List<string>();

    private HttpClientManager _httpClientManager;
    private ConfigManager _configManager;
    private UIManager _uiManager;
    private ChatSessionManager _chatSessionManager;
    private StreamProcessor _streamProcessor;
    

    void Start()
    {
        _configManager = new ConfigManager();

        _chatApiUrl = $"{_configManager.Domain}/stream-api.php";
        _httpClientManager = new HttpClientManager(sessionCookies, _chatApiUrl);

        _uiManager = new UIManager();
        _uiManager.Initialize(inputField, responseInputField, ScrollRect);

        _chatSessionManager = new ChatSessionManager(_uiManager);
        _streamProcessor = new StreamProcessor(_uiManager, _chatSessionManager);

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

            StartCoroutine(_streamProcessor.ProcessResponseStream(streamTask.Result));
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
}
