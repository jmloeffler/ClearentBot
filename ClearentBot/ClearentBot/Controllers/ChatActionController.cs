using ClearentBot.Models;
using Newtonsoft.Json;
using System.Configuration;
using System.Linq;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http;

namespace ClearentBot.Controllers
{
    public class ChatActionController : ApiController
    {
        public async Task<IHttpActionResult> Post(FormDataCollection post)
        {
            var chatAction = JsonConvert.DeserializeObject<ChatAction>(post["payload"]);
            if (chatAction.token != ConfigurationManager.AppSettings["SlackValidationToken"])
            {
                return Unauthorized();
            }
            
            if (chatAction.actions.First().value == "approver")
            {
                var workflow = await ChangeRequestWorkflow.Retrieve(chatAction.callback_id);
                workflow.AddApprover($"<@{chatAction.user.id}|{chatAction.user.name}>");
                await workflow.Save();

                return Ok(workflow.GetSlackResponse());
            }

            if (chatAction.actions.First().value == "disapprover")
            {
                var workflow = await ChangeRequestWorkflow.Retrieve(chatAction.callback_id);
                workflow.HeldBy = $"<@{chatAction.user.id}|{chatAction.user.name}>";
                await workflow.Save();

                return Ok(workflow.GetSlackResponse());
            }

            return Ok();
        }
    }
}
