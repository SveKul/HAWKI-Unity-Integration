using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

public class AuthenticationService
{
    private string _authUrl;
    private HttpClient _httpClient;
    private LoginDebugLogger _loginDebugLogger;

    public AuthenticationService(string domain, LoginDebugLogger loginDebugLogger)
    {
        this._loginDebugLogger = loginDebugLogger;
        _authUrl = $"{domain}/login.php";
        InitializeHttpClient();
    }

    private void InitializeHttpClient()
    {
        var httpClientHandler = new HttpClientHandler
        {
            UseCookies = true,
            AllowAutoRedirect = false,
            CookieContainer = new CookieContainer()
        };

        _httpClient = new HttpClient(httpClientHandler);
    }

    public async Task<bool> Login(string username, string password)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(_authUrl);

        if (!response.IsSuccessStatusCode)
        {
            _loginDebugLogger.Log(string.Format(LocalizationManager.GetLocalizedText(TextKey.FailedToInitializeSession), response.ReasonPhrase));
            return false;
        }

        if (!response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> cookies))
        {
            _loginDebugLogger.Log(LocalizationManager.GetLocalizedText(TextKey.FailedToObtainSessionCookies));
            return false;
        }

        return await MakeLoginRequest(username, password, cookies);
    }

    private async Task<bool> MakeLoginRequest(string username, string password, IEnumerable<string> cookies)
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
        request.Headers.Add("Origin", _authUrl);
        request.Headers.Referrer = new Uri(_authUrl);

        HttpResponseMessage response = await _httpClient.SendAsync(request);

        if (response.StatusCode == HttpStatusCode.Found)
        {
            var locationHeader = response.Headers.Location.ToString();
            if (locationHeader.EndsWith("interface.php"))
            {
                foreach (var cookie in cookies)
                {
                    ChatManager.sessionCookies.Add(cookie);
                }
                _loginDebugLogger.Log(LocalizationManager.GetLocalizedText(TextKey.LoginSuccessful));
                return true;
            }
            _loginDebugLogger.Log(LocalizationManager.GetLocalizedText(TextKey.LoginFailedRedirectingToLogin));
            return false;
        }

        _loginDebugLogger.Log(LocalizationManager.GetLocalizedText(TextKey.LoginFailed));
        return false;
    }
}
