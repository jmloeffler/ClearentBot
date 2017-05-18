using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClearentBot.Models
{
    public class SlackPost
    {
        public string token;
        public string team_id;
        public string team_domain;
        public string channel_id;
        public string channel_name;
        public string user_id;
        public string user_name;
        public string command;
        public string text;
        public string response_url;
    }
}