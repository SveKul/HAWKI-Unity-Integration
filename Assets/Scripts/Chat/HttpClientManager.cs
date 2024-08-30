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