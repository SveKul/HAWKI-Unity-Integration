using System.Collections.Generic;

public class ChatSessionManager
{
    private List<object> _chatMessages = new List<object>();
    private readonly UIManager _uiManager;

    public ChatSessionManager(UIManager uiManager)
    {
        _uiManager = uiManager;
    }

    public void AddUserMessage(string message)
    {
        _chatMessages.Add(new { role = "user", content = message });
    }

    public void AddAssistantMessage(string message)
    {
        _chatMessages.Add(new { role = "assistant", content = message });
    }

    public void ClearSession()
    {
        _chatMessages.Clear();
        _uiManager.ResetUI();
    }

    public List<object> GetChatMessages()
    {
        return _chatMessages;
    }
}