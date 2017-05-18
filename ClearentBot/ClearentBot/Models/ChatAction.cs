using System.Collections.Generic;

namespace ClearentBot.Models
{
    public class ChatAction
    {
        public List<SlackAction> actions;
        public string callback_id;
        public Team team;
        public Channel channel;
        public User user;
        public string token;
        public string response_url;
    }
}
