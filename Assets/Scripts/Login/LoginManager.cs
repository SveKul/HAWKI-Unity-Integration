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

using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField usernameInputField;
    public TMP_InputField passwordInputField;
    public Button loginButton;
    public TextMeshProUGUI debugText;

    private AuthenticationService _authService;

    void Start()
    {
        // Set the current language; this could be dynamically set based on user preference
        LocalizationManager.CurrentLanguage = "German";

        ConfigManager configManager = new ConfigManager();
        string domain = configManager.Domain;

        _authService = new AuthenticationService(domain);


        loginButton.onClick.AddListener(OnLoginButtonClicked);
        usernameInputField.onSubmit.AddListener(async delegate { await OnInputFieldSubmit(); });
        passwordInputField.onSubmit.AddListener(async delegate { await OnInputFieldSubmit(); });
    }

    async void OnLoginButtonClicked()
    {
        await TryLogin();
    }

    async Task OnInputFieldSubmit()
    {
        await TryLogin();
    }

    async Task TryLogin()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;

        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            bool success = await _authService.Login(username, password);
            if (success)
            {
                SceneManager.LoadScene("ChatScene");
            }
        }
        else
        {
            Debug.Log(LocalizationManager.GetLocalizedText(TextKey.PleaseEnterUsernameAndPassword));
        }
    }
}