using Newtonsoft.Json;
using RestSharp;
using SixtyThreeBits.Core.Infrastructure.Base;
using SixtyThreeBits.Core.Infrastructure.DTO;
using SixtyThreeBits.Core.Infrastructure.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Services
{
    public class MicrosoftGraphServices : RestApiServiceBase
    {
        #region Properties
        readonly string _tenant;
        readonly string _clientID;
        readonly string _clientSecret;
        readonly string _microsoftUserID;

        static string _accessToken;
        static DateTime? _accessTokenExpirationDate;

        const string _baseUrlToken = "https://login.microsoftonline.com/";
        const string _baseUrl = "https://graph.microsoft.com/v1.0/";
        #endregion

        #region Constructors
        public MicrosoftGraphServices(string tenant, string clientID, string clientSecret, string microsoftUserID) : base(baseUrl: _baseUrl)
        {
            _tenant = tenant;
            _clientID = clientID;
            _clientSecret = clientSecret;
            _microsoftUserID = microsoftUserID;
        }
        #endregion

        #region Methods        
        async Task InitAccessToken()
        {
            if (_accessTokenExpirationDate == null || _accessTokenExpirationDate.Value < DateTime.Now)
            {
                SetBaseUrl(_baseUrlToken);
                var resource = $"{_tenant}/oauth2/v2.0/token";

                var headers = new List<Parameter>
                {
                    new (Key: "Content-Type", Value: "application/x-www-form-urlencoded")
                };

                var parameters = new List<Parameter>
                {
                    new (Key: "client_id", Value: _clientID),
                    new (Key: "client_secret", Value: _clientSecret),
                    new (Key: "grant_type", Value: "client_credentials"),
                    new (Key: "scope", Value: "https://graph.microsoft.com/.default")
                };

                var executeResult = await ExecuteAsyncTask(
                    resource: resource,
                    method: Method.Post,
                    headers: headers,
                    parameters: parameters,                    
                    httpStatusCodeSuccess: Enums.HttpStatusCodes.Status200OK
                );                              
                
                if(executeResult.IsSuccess)
                {
                    var tokenResponse = executeResult.ResponseContent.DeserializeJsonTo<TokenResponse>();
                    if (tokenResponse != null)
                    {
                        _accessToken = tokenResponse.AccessToken;
                        _accessTokenExpirationDate = DateTime.Now.AddSeconds(tokenResponse.ExipresIn ?? 0);
                    }
                }
            }
        }

        static void ResetAccessToken()
        {
            _accessToken = null;
            _accessTokenExpirationDate = null;
        }

        public async Task<SendEmailResult> SendEmail(string emailTo, string subject, string body, string replyTo, IEnumerable<EmailAttachmentDTO> attachments, IEnumerable<string> cc, IEnumerable<string> bcc)
        {
            await InitAccessToken();

            SetBaseUrl(_baseUrl);
            var resource = $"users/{_microsoftUserID}/sendMail";

            var headers = new List<Parameter>
            {
                new (Key:"Content-type", Value: "application/json"),
                new (Key:"Authorization", Value: $"Bearer {_accessToken}"),
            };
                        
            var RequestBody = new MailMessageRequestBody();
            RequestBody.Message.Subject = subject;
            RequestBody.Message.Body.Content = body;
            RequestBody.Message.Body.ContentType = "html";


            if (!string.IsNullOrWhiteSpace(emailTo))
            {
                RequestBody.Message.ToRecipients = [new() { EmailAddress = new() { Address = emailTo } }];

                if (!string.IsNullOrWhiteSpace(replyTo))
                {
                    RequestBody.Message.ReplyTo = [new() { EmailAddress = new() { Address = replyTo } }];
                }
                if (cc?.Any() == true)
                {
                    RequestBody.Message.CCRecipients = cc.Select(Item => new MailMessageRequestBody.MessageClass.CCRecipientClass { EmailAddress = new() { Address = Item } }).ToList();
                }
                if (bcc?.Any() == true)
                {
                    RequestBody.Message.BCCRecipients = bcc.Select(Item => new MailMessageRequestBody.MessageClass.BCCRecipientClass { EmailAddress = new() { Address = Item } }).ToList();
                }

                if (attachments?.Any() == true)
                {
                    RequestBody.Message.Attachments = attachments.Select(Item => new MailMessageRequestBody.MessageClass.AttachmentClass
                    {
                        Name = Item.Filename,
                        ContentBytesBase64 = Convert.ToBase64String(Item.FileBytes)
                    }).ToList();
                }
            }

            var executeResult = await ExecuteAsyncTask(
                resource: resource,
                method: Method.Post,
                headers: headers,
                parameters: null,
                body: RequestBody.ToJson(),
                files: null,
                httpStatusCodeSuccess: Enums.HttpStatusCodes.Status202Accepted
            );
            var result = new SendEmailResult(executeResult);

            if(result.IsSuccess)
            {
                
            }
            else
            {
                ResetAccessToken();
            }

            return result;
        }
        #endregion

        #region Nested Classes
        class TokenResponse
        {
            #region Properties
            [JsonProperty("token_type")]
            public string TokeType { get; set; }

            [JsonProperty("expires_in")]
            public int? ExipresIn { get; set; }

            [JsonProperty("ext_expires_in")]
            public int? ExtExpiresIn { get; set; }

            [JsonProperty("access_token")]
            public string AccessToken { get; set; }
            #endregion
        }

        class MailMessageRequestBody
        {
            #region Properties
            [JsonProperty("message")]
            public MessageClass Message { get; set; } = new MessageClass();
            [JsonProperty("saveToSentItems")]
            public bool SaveToSentItems { get; set; } = false;
            #endregion

            #region Sub Classes
            public class MessageClass
            {
                #region Properties
                [JsonProperty("subject")]
                public string Subject { get; set; }

                [JsonProperty("body")]
                public BodyClass Body { get; set; } = new BodyClass();

                [JsonProperty("toRecipients")]
                public List<RecipientClass> ToRecipients { get; set; }

                [JsonProperty("replyTo")]
                public List<ReplyToClass> ReplyTo { get; set; }

                [JsonProperty("ccRecipients")]
                public List<CCRecipientClass> CCRecipients { get; set; }

                [JsonProperty("bccRecipients")]
                public List<BCCRecipientClass> BCCRecipients { get; set; }

                [JsonProperty("attachments")]
                public List<AttachmentClass> Attachments { get; set; }
                #endregion

                #region Sub Classes
                public class BodyClass
                {
                    #region Properties
                    [JsonProperty("contentType")]
                    public string ContentType { get; set; }
                    [JsonProperty("content")]
                    public string Content { get; set; }
                    #endregion
                }

                public class RecipientClass
                {
                    #region Properties
                    [JsonProperty("emailAddress")]
                    public EmailAddressClass EmailAddress { get; set; }
                    #endregion
                }

                public class ReplyToClass
                {
                    #region Properties
                    [JsonProperty("emailAddress")]
                    public EmailAddressClass EmailAddress { get; set; }
                    #endregion
                }

                public class CCRecipientClass
                {
                    #region Properties
                    [JsonProperty("emailAddress")]
                    public EmailAddressClass EmailAddress { get; set; }
                    #endregion
                }

                public class BCCRecipientClass
                {
                    #region Properties
                    [JsonProperty("emailAddress")]
                    public EmailAddressClass EmailAddress { get; set; }
                    #endregion
                }

                public class EmailAddressClass
                {
                    #region Properties
                    [JsonProperty("address")]
                    public string Address { get; set; }
                    #endregion
                }

                public class AttachmentClass
                {
                    #region Properties
                    [JsonProperty("@odata.type")]
                    public string ODataType { get; } = "#microsoft.graph.fileAttachment";
                    [JsonProperty("name")]
                    public string Name { get; set; }
                    [JsonProperty("contentType")]
                    public string ContentType { get; set; }
                    [JsonProperty("contentBytes")]
                    public string ContentBytesBase64 { get; set; }
                    #endregion
                }
                #endregion
            }
            #endregion
        }

        public class SendEmailResult : ApiResultBase
        {
            #region Constructors
            public SendEmailResult(ApiResultBase resultBase) : base(resultBase) { } 
            #endregion
        }
        #endregion
    }
}
