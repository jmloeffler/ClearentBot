using ClearentBot.Models;
using System.Configuration;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http;

namespace ClearentBot.Controllers
{
    public class ChangeRequestController : ApiController
    {
        public async Task<IHttpActionResult> Post(FormDataCollection formData)
        {
            var post = ParseFormData(formData);

            if (post.token != ConfigurationManager.AppSettings["SlackValidationToken"])
            {
                return Unauthorized();
            }

            if (post.command == "/approve")
            {
                var workflow = new ChangeRequestWorkflow();
                workflow.Description = post.text;
                workflow.Requestor = $"<@{post.user_id}|{post.user_name}>";
                workflow.Id = System.Guid.NewGuid();

                await workflow.Save();

                return Ok(workflow.GetSlackResponse());
            }

            return Ok(new SlackResponse("I did not understand"));
        }

        private static SlackPost ParseFormData(FormDataCollection formData)
        {
            //extract the url-encoded form parameters
            var token = formData["token"];
            var command = formData["command"];
            var channel_id = formData["channel_id"];
            var channel_name = formData["channel_name"];
            var text = formData["text"];
            var user_name = formData["user_name"];
            var user_id = formData["user_id"];
            var response_url = formData["response_url"];

            //route the request to the appropriate method
            return new SlackPost()
            {
                token = token,
                command = command,
                channel_id = channel_id,
                channel_name = channel_name,
                text = text,
                user_id = user_id,
                user_name = user_name,
                response_url = response_url
            };
        }


    }
}
