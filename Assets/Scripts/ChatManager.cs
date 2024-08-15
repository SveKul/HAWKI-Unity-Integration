﻿using System.Collections;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class ChatManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button sendButton;
    public TMP_InputField responseInputField;

    private string _chatApiUrl;
    public static List<string> sessionCookies = new List<string>();

    private HttpClient _httpClient;
    private string _domain;
    private string _model;

    // List to store all chat messages
    private List<object> _chatMessages = new List<object>();

    private Stopwatch stopwatch;

    void Start()
    {
        stopwatch = new Stopwatch();

        ConfigLoader configLoader = new ConfigLoader();
        _domain = configLoader.LoadDomainFromConfig();
        _model = configLoader.LoadModelFromConfig();
        _chatApiUrl = $"{_domain}/stream-api.php";
        stopwatch.Stop();

        InitializeHttpClient();
        sendButton.onClick.AddListener(OnSendButtonClicked);

        // Ensure the responseInputField is not interactable directly by the user
        responseInputField.readOnly = true;
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

    void OnSendButtonClicked()
    {
        string message = inputField.text;

        if (!string.IsNullOrEmpty(message))
        {
            responseInputField.text += "\nWaiting for response...";
            StartCoroutine(SendMessageToChatBot(message));
        }
    }

    IEnumerator SendMessageToChatBot(string message)
    {
        // Add the new user message to the list.
        _chatMessages.Add(new { role = "user", content = message });

        var requestObject = new
        {
            model = _model,
            stream = true,
            messages = _chatMessages
        };
        stopwatch.Start();
        string json = JsonConvert.SerializeObject(requestObject);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var responseTask = _httpClient.PostAsync(_chatApiUrl, content);
        stopwatch.Stop();
        Debug.Log($"Elapsed Time: {stopwatch.ElapsedMilliseconds} ms seriali");
        yield return new WaitUntil(() => responseTask.IsCompleted);

        if (responseTask.Result.IsSuccessStatusCode)
        {
            var streamTask = responseTask.Result.Content.ReadAsStreamAsync();
            yield return new WaitUntil(() => streamTask.IsCompleted);
            StartCoroutine(ProcessResponseStream(streamTask.Result));
        }
        else
        {
            var errorResponseTask = responseTask.Result.Content.ReadAsStringAsync();
            yield return new WaitUntil(() => errorResponseTask.IsCompleted);
            Debug.LogError("Error Response: " + errorResponseTask.Result);
            responseInputField.text += "\nError: " + responseTask.Result.ReasonPhrase;
        }
    }

    IEnumerator ProcessResponseStream(Stream responseStream)
    {
        StringBuilder responseContent = new StringBuilder();

        using (var reader = new StreamReader(responseStream))
        {
            while (!reader.EndOfStream)
            {
                stopwatch.Start();
                Task<string> readLineTask = reader.ReadLineAsync();
                yield return new WaitUntil(() => readLineTask.IsCompleted);
                stopwatch.Stop();
                Debug.Log($"Elapsed Time: {stopwatch.ElapsedMilliseconds} ms ReadLine");

                string line = readLineTask.Result;
                if (line == null)
                {
                    break;
                }

                if (line.StartsWith("data: "))
                {
                    stopwatch.Start();
                    line = line.Substring(6).Trim(); // Remove "data: " part

                    // Check if line is "[]" or empty
                    if (line == "[]" || string.IsNullOrEmpty(line))
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
                            }
                        }
                    }
                    catch (JsonSerializationException e)
                    {
                        Debug.LogError("JSON Deserialization error: " + e.Message);
                    }
                    stopwatch.Stop();
                    Debug.Log($"Elapsed Time: {stopwatch.ElapsedMilliseconds} ms DeSeri");
                }

                yield return null; // Ensure the UI updates after processing each line
            }

            // Update the responseInputField text after all chunks are processed
            responseInputField.text = responseInputField.text.Replace("\nWaiting for response...", "") + "\n" + responseContent.ToString() + "\n\n";

            // Adding AI response to chatMessages list
            _chatMessages.Add(new { role = "assistant", content = responseContent.ToString() });
        }
    }

    // Classes for response chunk
    private class ResponseChunk
    {
        public string id { get; set; }
        public string @object { get; set; }
        public int created { get; set; }
        public string model { get; set; }
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
            public string content { get; set; }
        }
    }
}
