using System.Collections.Generic;

public static class LocalizationManager
{
    private static readonly Dictionary<TextKey, string> EnglishTexts = new Dictionary<TextKey, string>
    {
        { TextKey.PleaseEnterUsernameAndPassword, "Please enter both username and password."},
        { TextKey.FailedToInitializeSession, "There was a problem starting your session: {0}"},
        { TextKey.LoginSuccessful, "Login successful. Redirecting to interface."},
        { TextKey.FailedToObtainSessionCookies, "Failed to obtain session cookies."},
        { TextKey.LoginFailedRedirectingToLogin, "Login failed. Redirecting back to login."},
        { TextKey.LoginFailed, "Login failed."},
        { TextKey.Welcome, "Welcome!" },
        { TextKey.Username, "Username" },
        { TextKey.Password, "Password" },
        { TextKey.Login, "Login" },
        { TextKey.AssistInitialResponse, "How may I help?" },
        { TextKey.RequestInitialPlaceholder, "You can enter your request here..." },
        { TextKey.Send, "Send" },
        { TextKey.WaitingForResponse, "Waiting for response...\n" }
    };

    private static readonly Dictionary<TextKey, string> GermanTexts = new Dictionary<TextKey, string>
    {
        { TextKey.PleaseEnterUsernameAndPassword, "Bitte geben Sie sowohl Benutzernamen als auch Passwort ein."},
        { TextKey.FailedToInitializeSession, "Leider konnte Ihre Sitzung nicht gestartet werden: {0}"},
        { TextKey.LoginSuccessful, "Login erfolgreich. Weiterleitung zur Schnittstelle."},
        { TextKey.FailedToObtainSessionCookies, "Konnte Sitzungscookies nicht erhalten."},
        { TextKey.LoginFailedRedirectingToLogin, "Login fehlgeschlagen. Zurück zum Anmeldebildschirm."},
        { TextKey.LoginFailed, "Login fehlgeschlagen."},
        { TextKey.Welcome, "Willkommen!" },
        { TextKey.Username, "Benutzername" },
        { TextKey.Password, "Passwort" },
        { TextKey.Login, "Login" },
        { TextKey.AssistInitialResponse, "Wie kann ich helfen?" },
        { TextKey.RequestInitialPlaceholder, "Hier kannst Du deine Anfrage stellen..." },
        { TextKey.Send, "Senden" },
        { TextKey.WaitingForResponse, "Warte auf Antwort...\n" }
    };

    private static string _currentLanguage = "German"; // Default language
    public static string CurrentLanguage 
    { 
        get => _currentLanguage; 
        set
        {
            if (_currentLanguage != value)
            {
                _currentLanguage = value;
                NotifyLanguageChanged();
            }
        }
    }

    private static List<ILocalizationObserver> observers = new List<ILocalizationObserver>();

    public static void Subscribe(ILocalizationObserver observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }
    }

    public static void Unsubscribe(ILocalizationObserver observer)
    {
        if (observers.Contains(observer))
        {
            observers.Remove(observer);
        }
    }

    private static void NotifyLanguageChanged()
    {
        foreach (var observer in observers)
        {
            observer.OnLanguageChanged();
        }
    }

    public static string GetLocalizedText(TextKey key)
    {
        switch (CurrentLanguage)
        {
            case "German":
                return GermanTexts[key];
            case "English":
            default:
                return EnglishTexts[key];
        }
    }
}

public interface ILocalizationObserver
{
    void OnLanguageChanged();
}

