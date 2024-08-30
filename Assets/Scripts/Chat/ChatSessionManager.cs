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