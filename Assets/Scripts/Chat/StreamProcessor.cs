using System.Collections;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class StreamProcessor
{
    private readonly UIManager _uiManager;
    private readonly ChatSessionManager _chatSessionManager;

    public StreamProcessor(UIManager uiManager, ChatSessionManager chatSessionManager)
    {
        _uiManager = uiManager;
        _chatSessionManager = chatSessionManager;
    }

    public IEnumerator ProcessResponseStream(Stream responseStream)
    {
        StringBuilder responseContent = new StringBuilder();

        using (var reader = new StreamReader(responseStream))
        {
            while (!reader.EndOfStream)
            {
                Task<string> readLineTask = reader.ReadLineAsync();
                yield return new WaitUntil(() => readLineTask.IsCompleted);

                string line = readLineTask.Result;
                if (line == null)
                {
                    break;
                }

                if (line.StartsWith("data: "))
                {
                    line = line.Substring(6).Trim(); // Remove "data: " part

                    if (line == "[]" || string.IsNullOrEmpty(line))
                    {
                        break;
                    }

                    if (line.StartsWith("["))
                    {
                        continue;
                    }

                    try
                    {
                        var chunk = JsonConvert.DeserializeObject<ResponseChunk>(line);
                        if (chunk != null && chunk.choices.Count > 0 && chunk.choices[0].delta != null)
                        {
                            if (chunk.choices[0].delta.content != null)
                            {
                                responseContent.Append(chunk.choices[0].delta.content);
                                _uiManager.AddMessageToResponse(chunk.choices[0].delta.content);
                            }
                        }
                    }
                    catch (JsonSerializationException e)
                    {
                        Debug.LogError("JSON Deserialization error: " + e.Message);
                    }
                }

                yield return null;
            }

            _chatSessionManager.AddAssistantMessage(responseContent.ToString());

            _uiManager.AddMessageToResponse("\n\n");
        }
    }
}