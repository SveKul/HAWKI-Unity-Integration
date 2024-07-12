using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField usernameInputField;
    public TMP_InputField passwordInputField;
    public Button loginButton;

    private string domain;
    private string authUrl;
    private string interfaceUrl;

    private HttpClientHandler httpClientHandler;
    private HttpClient httpClient;

    void Start()
    {
        LoadDomainFromConfig();
        loginButton.onClick.AddListener(OnLoginButtonClicked);
    }

    void LoadDomainFromConfig()
    {
        try
        {
            // Read the configuration file
            string configPath = Path.Combine(Application.dataPath, "config.txt");
            string[] configLines = File.ReadAllLines(configPath);

            foreach (string line in configLines)
            {
                if (line.StartsWith("Domain="))
                {
                    domain = line.Substring("Domain=".Length);
                    break;
                }
            }

            if (string.IsNullOrEmpty(domain))
            {
                Debug.LogError("Domain not found in config file!");
                return;
            }

            authUrl = $"{domain}/login.php";
            interfaceUrl = $"{domain}/interface.php";
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to read config file: " + e.Message);
        }
    }

    void InitializeHttpClient()
    {
        httpClientHandler = new HttpClientHandler()
        {
            UseCookies = true,
            AllowAutoRedirect = false, // This allows us to handle the redirect ourselves
            CookieContainer = new CookieContainer()
        };
        httpClient = new HttpClient(httpClientHandler);
    }

    async void OnLoginButtonClicked()
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
        HttpResponseMessage response = await httpClient.GetAsync(authUrl);

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
                // Further process the successful login, e.g., redirect to interface
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

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, authUrl);
        request.Content = formData;

        // Set headers
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
        request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("de-DE", 0.9));
        request.Headers.Add("DNT", "1");
        request.Headers.Add("Upgrade-Insecure-Requests", "1");
        request.Headers.Add("Origin", domain);
        request.Headers.Referrer = new Uri(authUrl);

        HttpResponseMessage response = await httpClient.SendAsync(request);

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