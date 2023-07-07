using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using SixtyThreeBits.Core.Abstractions;
using SixtyThreeBits.Libraries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Services
{
    public class Mailgun : SixtyThreeBitsDataObject, IEmail
    {
        #region Properties
        string BaseUrl;
        string MailgunApiKey;
        string MailgunDomain;
        string MailgunFrom;
        #endregion

        #region Constructors        
        public Mailgun(string BaseUrl, string MailgunApiKey, string MailgunDomain, string MailgunFrom)
        {
            this.BaseUrl = BaseUrl;
            this.MailgunApiKey = MailgunApiKey;
            this.MailgunDomain = MailgunDomain;
            this.MailgunFrom = MailgunFrom;
        }
        #endregion

        #region Methods
        public async Task<EmailSendResult> Send(string To, string Subject, string Body, string ReplyTo = null, IEnumerable<EmailAttachment> Attachments = null, IEnumerable<string> CCs = null, IEnumerable<string> BCCs = null)
        {

            var Result = new EmailSendResult();
            var Client = new RestClient(BaseUrl);
            
            var Request = new RestRequest();
            Request.Authenticator = new HttpBasicAuthenticator("api", MailgunApiKey);
            Request.AddParameter("o:dkim", "true");
            Request.AddParameter("domain", MailgunDomain, ParameterType.UrlSegment);
            Request.Resource = $"{MailgunDomain}/messages";
            Request.AddParameter("from", MailgunFrom);
            Request.AddParameter("to", To);
            Request.AddParameter("subject", Subject);
            Request.AddParameter("html", Body);

            if (!string.IsNullOrWhiteSpace(ReplyTo))
            {
                Request.AddHeader("h:Reply-To", ReplyTo);
            }

            if (Attachments != null)
            {
                foreach (var Item in Attachments)
                {
                    Request.AddFile("attachment", Item.FileBytes, Item.Filename, "application/octet-stream");
                }
            }
            if (CCs != null)
            {
                foreach (var Item in CCs)
                {
                    Request.AddParameter("cc", Item);
                }
            }
            if (BCCs != null)
            {
                foreach (var Item in BCCs)
                {
                    Request.AddParameter("bcc", Item);
                }
            }

            Request.Method = Method.Post;
            var Response = await Client.ExecuteAsync(Request);

            var ResponseData = Response.Content.DeserializeJsonTo<MailgunResponse>();

            if (ResponseData?.EmailMessageID != null)
            {
                Result.EmailMessageID = ResponseData.EmailMessageID.Trim('<').Trim('>');
                Result.IsSent = true;
            }
            Result.ResponseContent = Response.Content;

            return Result;
        }
        #endregion

        #region Sub Classes     
        class MailgunResponse
        {
            #region Properties
            [JsonProperty("id")]
            public string EmailMessageID { get; set; }
            [JsonProperty("message")]
            public string Message { get; set; }
            #endregion
        }

        public class WebhookEvents
        {
            #region Properties
            public const string Delivered = "delivered";
            public const string Clicked = "clicked";
            public const string Opened = "opened";
            public const string Failed = "failed";
            public const string Spammed = "complained";
            public const string Unsubscribed = "unsubscribed";
            #endregion
        }
        #endregion
    }

    public class MailgunWebhookResult
    {
        #region Properties
        public string Timestamp { get; set; }
        public string Token { get; set; }
        public string Signature { get; set; }
        public string Event { get; set; }
        public string MessageID { get; set; }
        public string UserIP { get; set; }
        public string UrlClicked { get; set; }
        public string UserBrowser { get; set; }
        public string Json { get; set; }
        public bool HasData { get; set; }
        #endregion
    }


    public class MailgunWebhookResponse
    {
        #region Properties
        [JsonProperty("signature")]
        public SignatureItem Signature { get; set; }

        [JsonProperty("event-data")]
        public EventDataItem EventData { get; set; }
        #endregion

        #region Sub Classes
        public class SignatureItem
        {
            #region Properties
            [JsonProperty("timestamp")]
            public string Timestamp { get; set; }
            [JsonProperty("token")]
            public string Token { get; set; }
            [JsonProperty("signature")]
            public string Signature { get; set; }
            #endregion
        }

        public class EventDataItem
        {
            #region Properties
            [JsonProperty("event")]
            public string Event { get; set; }

            [JsonProperty("message")]
            public MessageItem Message { get; set; }

            [JsonProperty("ip")]
            public string IP { get; set; }

            [JsonProperty("url")]
            public string Url { get; set; }

            [JsonProperty("client-info")]
            public ClientInfoItem ClientInfo { get; set; }
            #endregion

            #region Sub Classes
            public class MessageItem
            {
                #region Properties
                [JsonProperty("headers")]
                public HeadersItem Headers { get; set; }
                #endregion

                #region Sub Classes
                public class HeadersItem
                {
                    #region Properties
                    [JsonProperty("message-id")]
                    public string MessageID { get; set; }
                    #endregion
                }
                #endregion
            }

            public class ClientInfoItem
            {
                #region Properties
                [JsonProperty("client-name")]
                public string ClientName { get; set; }
                [JsonProperty("client-type")]
                public string ClientType { get; set; }
                [JsonProperty("user-agent")]
                public string UserAgent { get; set; }
                [JsonProperty("device-type")]
                public string DeviceType { get; set; }
                [JsonProperty("client-os")]
                public string ClientOs { get; set; }
                #endregion
            }
            #endregion
        }
        #endregion
    }
}