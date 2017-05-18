using System.Collections.Generic;

namespace ClearentBot.Models
{
    public class SlackResponse
    {
        public SlackResponse(string text) : this("ephemeral", text)
        { }

        public SlackResponse(string responseType, string text)
        {
            this.response_type = responseType;
            this.text = text;
        }

        public string response_type { get; set; }
        public string text { get; set; }
        public List<Attachment> attachments { get; set; }
    }
}
