using System.Collections.Generic;

namespace ClearentBot.Models
{
    public class Attachment
    {
        public string title;
        public string fallback;
        public string color;
        public string attachment_type;
        public string callback_id;
        public List<SlackAction> actions;
    }
}
