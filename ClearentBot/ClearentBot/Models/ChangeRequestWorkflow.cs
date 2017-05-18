using ClearentBot.Controllers;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClearentBot.Models
{
    public class ChangeRequestWorkflow
    {
        public Guid Id;
        public string Requestor;
        public string Description;
        public string Approver1;
        public string Approver2;
        public string Approver3;
        public string HeldBy;

        public bool Complete
        {
            get { return (Approver1 != null && Approver2 != null && Approver3 != null) || (HeldBy != null); }
        }

        public SlackResponse GetSlackResponse()
        {
            var response = new SlackResponse("in_channel", $"{Requestor} would like to deploy: {Description}.");
            response.attachments = new List<Attachment>();

            if (!Complete)
            {
                var approvers = new Attachment { title = "We need three approvals...", attachment_type = "default", fallback = "Approvers", callback_id = Id.ToString() };
                var action1 = new SlackAction { name = "Approver", text = "I Approve.", type = "button", value = "approver", style = "primary" };
                var action2 = new SlackAction { name = "Disapprover", text = "Hold up!", type = "button", value = "disapprover", style = "danger" };
                approvers.actions = new List<SlackAction> { action1, action2 };
                response.attachments.Add(approvers);
            }
            else if(HeldBy == null)
            {
                var clear = new Attachment { title = $"This change is cleared for deployment!", attachment_type = "default", fallback = $"This change is cleared for deployment!" };
                response.attachments.Add(clear);
            }

            if(Approver1 != null)
            {
                var approver = new Attachment { title = $":white_check_mark: Approved by {Approver1}", attachment_type = "default", fallback = $"Approved by {Approver1}" };
                response.attachments.Add(approver);
            }

            if (Approver2 != null)
            {
                var approver = new Attachment { title = $":white_check_mark: Approved by {Approver2}", attachment_type = "default", fallback = $"Approved by {Approver2}" };
                response.attachments.Add(approver);
            }

            if (Approver3 != null)
            {
                var approver = new Attachment { title = $":white_check_mark: Approved by {Approver3}", attachment_type = "default", fallback = $"Approved by {Approver3}" };
                response.attachments.Add(approver);
            }

            if (HeldBy != null)
            {
                var approver = new Attachment { title = $":no_entry: {HeldBy} has requested a hold!  Please converse and re-request MTP when ready.", attachment_type = "default", fallback = $"{HeldBy} has requested a hold!" };
                response.attachments.Add(approver);
            }

            return response;
        }

        public void AddApprover(string approver)
        {
            //don't allow duplicate approvers
            //if (Approver1 == approver || Approver2 == approver || Approver3 == approver) return;

            if (Approver1 == null) Approver1 = approver;
            else if (Approver2 == null) Approver2 = approver;
            else if (Approver3 == null) Approver3 = approver;
        }

        public static async Task<ChangeRequestWorkflow> Retrieve(string id)
        {
            var container = GetBlobContainer();
            var reference = container.GetBlockBlobReference(id);
            var jsonWorkflow = await reference.DownloadTextAsync();
            return JsonConvert.DeserializeObject<ChangeRequestWorkflow>(jsonWorkflow);
        }

        public async Task Save()
        {
            var container = GetBlobContainer();
            var reference = container.GetBlockBlobReference(this.Id.ToString());
            await reference.UploadTextAsync(JsonConvert.SerializeObject(this));
        }

        private static CloudBlobContainer GetBlobContainer()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("clearentbot_AzureStorageConnectionString"));
            var client = storageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference("change-requests");
            container.CreateIfNotExists();

            return container;
        }
    }
}