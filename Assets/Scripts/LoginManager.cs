using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DefaultNamespace;
using TMPro;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField usernameInputField;
    public TMP_InputField passwordInputField;
    public Button loginButton;

    private string _domain;
    private string _authUrl;
    private string _interfaceUrl;

    private HttpClientHandler _httpClientHandler;
    private HttpClient _httpClient;

    void Start()
    {
        ConfigLoader configLoader = new ConfigLoader();
        _domain = configLoader.LoadDomainFromConfig();
        _authUrl = $"{_domain}/login.php";
        _interfaceUrl = $"{_domain}/interface.php";
        
        loginButton.onClick.AddListener(OnLoginButtonClicked);
        
        // Add listeners for the Enter key on the input fields
        usernameInputField.onSubmit.AddListener(delegate { OnInputFieldSubmit(); });
        passwordInputField.onSubmit.AddListener(delegate { OnInputFieldSubmit(); });
    }
    
    void InitializeHttpClient()
    {
        _httpClientHandler = new HttpClientHandler()
        {
            UseCookies = true,
            AllowAutoRedirect = false, // This allows us to handle the redirect ourselves
            CookieContainer = new CookieContainer()
        };
        _httpClient = new HttpClient(_httpClientHandler);
    }

    async void OnLoginButtonClicked()
    {
        await TryLogin();
    }

    async void OnInputFieldSubmit()
    {
        await TryLogin();
    }

    async Task TryLogin()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;

        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            InitializeHttpClient(); // Initialize a new client for each login attempt

            await InitializeSessionAndLogin(username, password);
        }
        else
        {
            Debug.Log("Please enter both username and password.");
        }
    }

    async Task InitializeSessionAndLogin(string username, string password)
    {
        // Initialize session to obtain PHPSESSID
        HttpResponseMessage response = await _httpClient.GetAsync(_authUrl);

        if (!response.IsSuccessStatusCode)
        {
            Debug.LogError("Failed to initialize session: " + response.ReasonPhrase);
            return;
        }

        // Load cookies from response if available
        if (response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> cookies))
        {
            // Perform login with the obtained cookies
            bool loginSuccess = await MakeLoginRequest(username, password);

            if (loginSuccess)
            {
                Debug.Log("Login successful. Redirecting to interface.");

                // Save the cookies to be used later
                foreach (var cookie in cookies)
                {
                    ChatManager.sessionCookies.Add(cookie);
                }

                SceneManager.LoadScene("ChatScene"); // assuming "ChatScene" is the name of your new scene
            }
        }
        else
        {
            Debug.LogError("Failed to obtain session cookies.");
        }
    }

    async Task<bool> MakeLoginRequest(string username, string password)
    {
        var formData = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("account", username),
            new KeyValuePair<string, string>("password", password),
        });

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _authUrl);
        request.Content = formData;

        // Set headers
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
        request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("de-DE", 0.9));
        request.Headers.Add("DNT", "1");
        request.Headers.Add("Upgrade-Insecure-Requests", "1");
        request.Headers.Add("Origin", _domain);
        request.Headers.Referrer = new Uri(_authUrl);

        HttpResponseMessage response = await _httpClient.SendAsync(request);

        if (response.StatusCode == HttpStatusCode.Found) // Check for redirect
        {
            var locationHeader = response.Headers.Location.ToString();
            if (locationHeader.EndsWith("interface.php"))
            {
                return true; // Successful login
            }
            else if (locationHeader.EndsWith("login.php"))
            {
                Debug.Log("Login failed. Redirecting back to login.");
                return false; // Failed login
            }
        }

        Debug.LogError("Login failed.");
        return false;
    }
}