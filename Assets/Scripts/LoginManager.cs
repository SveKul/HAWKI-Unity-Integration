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
    public TextMeshProUGUI debugText; 

    private string _domain;
    private string _authUrl;
    private string _interfaceUrl;

    private HttpClientHandler _httpClientHandler;
    private HttpClient _httpClient;

    void Start()
    {
        // Set the current language; this could be dynamically set based on user preference
        LocalizationManager.CurrentLanguage = "German"; 

        ConfigLoader configLoader = new ConfigLoader();
        _domain = configLoader.LoadDomainFromConfig();
        _authUrl = $"{_domain}/login.php";
        _interfaceUrl = $"{_domain}/interface.php";
        
        loginButton.onClick.AddListener(OnLoginButtonClicked);
        
        usernameInputField.onSubmit.AddListener(delegate { OnInputFieldSubmit(); });
        passwordInputField.onSubmit.AddListener(delegate { OnInputFieldSubmit(); });
    }
    
    void InitializeHttpClient()
    {
        _httpClientHandler = new HttpClientHandler()
        {
            UseCookies = true,
            AllowAutoRedirect = false,
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
            InitializeHttpClient();
            await InitializeSessionAndLogin(username, password);
        }
        else
        {
            UpdateDebugText(LocalizationManager.GetLocalizedText(TextKey.PleaseEnterUsernameAndPassword));
        }
    }

    async Task InitializeSessionAndLogin(string username, string password)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(_authUrl);

        if (!response.IsSuccessStatusCode)
        {
            UpdateDebugText(string.Format(LocalizationManager.GetLocalizedText(TextKey.FailedToInitializeSession), response.ReasonPhrase));
            return;
        }

        if (response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> cookies))
        {
            bool loginSuccess = await MakeLoginRequest(username, password);

            if (loginSuccess)
            {
                UpdateDebugText(LocalizationManager.GetLocalizedText(TextKey.LoginSuccessful));

                foreach (var cookie in cookies)
                {
                    ChatManager.sessionCookies.Add(cookie);
                }

                SceneManager.LoadScene("ChatScene");
            }
        }
        else
        {
            UpdateDebugText(LocalizationManager.GetLocalizedText(TextKey.FailedToObtainSessionCookies));
        }
    }

    async Task<bool> MakeLoginRequest(string username, string password)
    {
        var formData = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("account", username),
            new KeyValuePair<string, string>("password", password),
        });

        var request = new HttpRequestMessage(HttpMethod.Post, _authUrl)
        {
            Content = formData
        };

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
        request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("de-DE", 0.9));
        request.Headers.Add("DNT", "1");
        request.Headers.Add("Upgrade-Insecure-Requests", "1");
        request.Headers.Add("Origin", _domain);
        request.Headers.Referrer = new Uri(_authUrl);

        HttpResponseMessage response = await _httpClient.SendAsync(request);

        if (response.StatusCode == HttpStatusCode.Found)
        {
            var locationHeader = response.Headers.Location.ToString();
            if (locationHeader.EndsWith("interface.php"))
            {
                return true;
            }
            else if (locationHeader.EndsWith("login.php"))
            {
                UpdateDebugText(LocalizationManager.GetLocalizedText(TextKey.LoginFailedRedirectingToLogin));
                return false;
            }
        }

        UpdateDebugText(LocalizationManager.GetLocalizedText(TextKey.LoginFailed));
        return false;
    }

    void UpdateDebugText(string message)
    {
        if (debugText != null)
        {
            debugText.text = message;
        }
        else
        {
            Debug.LogWarning("Debug TextMeshProUGUI is not assigned.");
        }
    }
}