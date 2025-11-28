using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using SixtyThreeBits.Core.Infrastructure.Services.Base;
using SixtyThreeBits.Core.Libraries.EmailClients.DTO;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Services
{
    public class MailgunService : RestApiServiceBase
    {
        #region Properties
        readonly string _mailgunDomain;
        readonly string _mailgunFrom;
        #endregion

        #region Constructors        
        public MailgunService(string baseUrl, string mailgunApiKey, string mailgunDomain, string mailgunFrom) : base(baseUrl: baseUrl, authenticator: new HttpBasicAuthenticator("api", mailgunApiKey))
        {
            _mailgunDomain = mailgunDomain;
            _mailgunFrom = mailgunFrom;
        }
        #endregion

        #region Methods
        public async Task<SendEmailResult> SendEmail(string emailTo, string subject, string body, string replyTo = null, IEnumerable<EmailAttachmentDTO> attachments = null, IEnumerable<string> cc = null, IEnumerable<string> bcc = null)
        {            
            var resource = $"{_mailgunDomain}/messages";

            var headers = new List<Parameter>();
            if (!string.IsNullOrWhiteSpace(replyTo))
            {
                headers.Add(new (Key: "h:Reply-To", Value: replyTo));
            }

            var parameters = new List<Parameter> 
            {
                new (Key: "domain", Value: "true"),
                new (Key: "o:dkim", Value: "true"),
                new (Key: "from", Value: _mailgunFrom),
                new (Key: "to", Value: emailTo),
                new (Key: "subject", Value: subject),
                new (Key: "html", Value: body),
            };            
            if (cc != null)
            {
                parameters.AddRange(cc.Select(item => new Parameter(Key: "cc", Value: item)));
            }
            if (bcc != null)
            {
                parameters.AddRange(bcc.Select(item => new Parameter(Key: "bcc", Value: item)));                
            }

            var files = new List<File>();
            if (attachments != null)
            {
                files.AddRange(attachments.Select(item => new File(Filename: item.Filename, FileBytes: item.FileBytes)));
            }

            var executeResult = await ExecuteAsyncTask(
                resource: resource,
                method: Method.Post,
                headers: headers,
                parameters: parameters,                
                files: files
            );
            var result = new SendEmailResult(executeResult);

            if (result.IsSuccess)
            {                
                var responseData = result.ResponseContent.DeserializeJsonTo<MailgunMessageSendResponse>();
                if (responseData?.EmailMessageID != null)
                {
                    result.EmailMessageID = responseData.EmailMessageID.Trim('<').Trim('>');                    
                }                
            }

            return result;
        }
        #endregion

        #region Nested Classes     
        class MailgunMessageSendResponse
        {
            #region Properties
            [JsonProperty("id")]
            public string EmailMessageID { get; set; }
            [JsonProperty("message")]
            public string Message { get; set; }
            #endregion
        }

        public class SendEmailResult(ApiResultBase resultBase) : ApiResultBase (resultBase)
        {
            #region Properties
            public string EmailMessageID { get; set; }
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

        #region Nested Classes
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

            #region Nested Classes
            public class MessageItem
            {
                #region Properties
                [JsonProperty("headers")]
                public HeadersItem Headers { get; set; }
                #endregion

                #region Nested Classes
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