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
    private LoginDebugLogger _loginDebugLogger;

    void Start()
    {
        // Set the current language; this could be dynamically set based on user preference
        LocalizationManager.CurrentLanguage = "German"; 
        
        ConfigManager configManager = new ConfigManager();
        string domain = configManager.Domain;
        
        _loginDebugLogger = new LoginDebugLogger(debugText);
        _authService = new AuthenticationService(domain, _loginDebugLogger);
        
        
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
            _loginDebugLogger.Log(LocalizationManager.GetLocalizedText(TextKey.PleaseEnterUsernameAndPassword));
        }
    }
}