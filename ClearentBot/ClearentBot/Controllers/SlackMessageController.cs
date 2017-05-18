using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Net;
using System.Web.Http;

namespace ClearentBot.Controllers
{
    public class SlackMessageController : ApiController
    {
        public IHttpActionResult Post(SlackEvent slackEvent)
        {
            if(slackEvent.token != ConfigurationManager.AppSettings["SlackValidationToken"])
            {
                return Unauthorized();
            }

            if(slackEvent.type == "event_callback")
            {
                if(slackEvent.@event.Value<string>("type") == "message")
                {
                    var message = slackEvent.@event.ToObject<SlackMessage>();
                    //invoke a web request to post a message back to the channel
                    var url = "https://slack.com/api/chat.postMessage";
                    var client = new WebClient();
                    client.BaseAddress = url;
                    client.QueryString.Add("token", ConfigurationManager.AppSettings["ClearentBotUserToken"]);
                    client.QueryString.Add("channel", message.channel);
                    client.QueryString.Add("text", $"Got your message.");
                    //client.UploadValues(url, "POST", client.QueryString);
                }

            }

            return Ok();
        }

        public class SlackEvent
        {
            public string token;
            public string team_id;
            public string api_app_id;
            public JObject @event;
            public string type;
            public string authed_users;
            public string event_id;
            public string event_time;
        }

        public class SlackMessage
        {
            public string type;
            public string channel;
            public string user;
            public string text;
            public string ts;
        }

        
    }
}
