using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class HttpClientManager
{
    private HttpClient _httpClient;
    private string _apiUrl;

    public HttpClientManager(List<string> sessionCookies, string apiUrl)
    {
        _apiUrl = apiUrl;
        InitializeHttpClient(sessionCookies);
    }

    private void InitializeHttpClient(List<string> sessionCookies)
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

    public async Task<HttpResponseMessage> PostAsync(string json, string apiUrl)
    {
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return await _httpClient.PostAsync(apiUrl, content);
    }

    public async Task<Stream> GetResponseStreamAsync(HttpResponseMessage response)
    {
        return await response.Content.ReadAsStreamAsync();
    }
}