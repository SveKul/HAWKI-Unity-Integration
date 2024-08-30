using System.Collections.Generic;

public class ResponseChunk
{
    public string id { get; set; }
    public string @object { get; set; }
    public int created { get; set; }
    public string model { get; set; }
    public List<Choice> choices { get; set; }

    public class Choice
    {
        public int index { get; set; }
        public Delta delta { get; set; }
        public object logprobs { get; set; }
        public string finish_reason { get; set; }
    }

    public class Delta
    {
        public string content { get; set; }
    }
}