using System;
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
        { TextKey.LoginFailed, "Login failed."}
    };

    private static readonly Dictionary<TextKey, string> GermanTexts = new Dictionary<TextKey, string>
    {
        { TextKey.PleaseEnterUsernameAndPassword, "Bitte geben Sie sowohl Benutzernamen als auch Passwort ein."},
        { TextKey.FailedToInitializeSession, "Leider konnte Ihre Sitzung nicht gestartet werden: {0}"},
        { TextKey.LoginSuccessful, "Login erfolgreich. Weiterleitung zur Schnittstelle."},
        { TextKey.FailedToObtainSessionCookies, "Konnte Sitzungscookies nicht erhalten."},
        { TextKey.LoginFailedRedirectingToLogin, "Login fehlgeschlagen. Zurück zum Anmeldebildschirm."},
        { TextKey.LoginFailed, "Login fehlgeschlagen."}
    };

    public static string CurrentLanguage { get; set; } = "English"; // Default language

    public static string GetLocalizedText(TextKey key)
    {
        switch (CurrentLanguage)
        {
            case "German":
                return GermanTexts[key];
            case "English":
                return EnglishTexts[key];
            default:
                return EnglishTexts[key];
        }
    }
}