using Microsoft.IdentityModel.Tokens;
using RestSharp;
using SixtyThreeBits.Core.Infrastructure.Services.Base;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Services
{
    public class GmailServiceBase : RestApiServiceBase
    {
        #region Properties
        const string _baseUrl = "https://oauth2.googleapis.com/token";
        protected readonly string _serviceAccountPrivateKey;
        protected readonly string _serviceAccountPrivateKeyId;
        protected readonly string _serviceAccountClientEmail;
        #endregion

        #region Constructors
        public GmailServiceBase(string serviceAccountPrivateKey, string serviceAccountPrivateKeyId, string serviceAccountClientEmail) : base(_baseUrl)
        {
            _serviceAccountPrivateKey = serviceAccountPrivateKey;
            _serviceAccountPrivateKeyId = serviceAccountPrivateKeyId;
            _serviceAccountClientEmail = serviceAccountClientEmail;
        }
        #endregion

        #region Methods
        protected async Task<string> GetAccessToken(string userEmail)
        {
            var jwToken = CreateServiceAccountJwt(userEmail);
            var accessToken = string.Empty;

            var restClient = new RestClient();
            var restRequest = new RestRequest(_baseUrl, Method.Post);
            restRequest.AddParameter("grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer");
            restRequest.AddParameter("assertion", UrlEncoder.Default.Encode(jwToken));
            var restResponse = await restClient.ExecuteAsync(restRequest);
            if (restResponse.IsSuccessful && !string.IsNullOrEmpty(restResponse.Content))
            {
                var responseContent = JsonSerializer.Deserialize<AccessTokenResponse>(restResponse.Content);
                if (string.IsNullOrEmpty(responseContent?.Error))
                {
                    accessToken = responseContent?.AccessToken;
                }
            }

            return accessToken;
        }

        protected string CreateServiceAccountJwt(string userEmail)
        {
            var allowedScopes = new List<string>
            {
                "https://www.googleapis.com/auth/admin.directory.user",
                "https://www.googleapis.com/auth/gmail.send",
                "https://mail.google.com/",
            };

            var tokenHandler = new JwtSecurityTokenHandler
            {
                SetDefaultTimesOnTokenCreation = false
            };

            using var rsa = RSA.Create(2048);

            rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(_serviceAccountPrivateKey), out _);
            var key = new RsaSecurityKey(rsa);
            var Now = DateTime.UtcNow;

            var token = tokenHandler.CreateJwtSecurityToken(new SecurityTokenDescriptor
            {
                SigningCredentials = new SigningCredentials(key: key, algorithm: SecurityAlgorithms.RsaSha256),
                Issuer = _serviceAccountClientEmail,
                Audience = _baseUrl,
                Subject = new ClaimsIdentity(new[]
                {
                new Claim("sub", userEmail),
                new Claim("scope", string.Join(" ", allowedScopes)),
            }),
                IssuedAt = Now,
                Expires = Now.AddSeconds(3600),
            });

            token.Header.Add("kid", _serviceAccountPrivateKeyId);

            return tokenHandler.WriteToken(token);
        }

        protected string GetRestErrorMessage(string errorMessage, string endpointUrl, Method method, HttpStatusCode statusCode, object requestBody = null, string responseBody = null)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(errorMessage);
            stringBuilder.AppendLine($"Request: {method.ToString()} {endpointUrl}");
            if (requestBody != null)
            {
                stringBuilder.AppendLine($"Body: {JsonSerializer.Serialize(requestBody)}\n");
            }
            stringBuilder.AppendLine($"Response: Code {(int)statusCode}");
            stringBuilder.AppendLine($"Content: {JsonSerializer.Serialize(responseBody)}");

            return stringBuilder.ToString();
        }
        #endregion

        #region Nested Classes
        class AccessTokenResponse
        {
            #region Properties
            [JsonPropertyName("error")]
            public string Error { get; set; }
            public string ErrorMessage { get; set; }
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; }
            #endregion
        }
        #endregion
    }

    public class GmailSenderService : GmailServiceBase
    {
        #region Fields
        readonly string _createUserAccountEmail;
        readonly string _sendEmailAccountEmail;

        const string AuthBaseUrl = "https://oauth2.googleapis.com/token";
        const string CreateUsersEndpoint = "https://admin.googleapis.com/admin/directory/v1/users";
        const string SendEmailEndpoint = "https://gmail.googleapis.com/upload/gmail/v1/users/me/messages/send";
        #endregion

        #region Constructors
        public GmailSenderService(string serviceAccountPrivateKey, string serviceAccountPrivateKeyId, string serviceAccountClientEmail, string sendEmailAccountEmail)
            : base(serviceAccountPrivateKey, serviceAccountPrivateKeyId, serviceAccountClientEmail)
        {
            _sendEmailAccountEmail = sendEmailAccountEmail;
        }
        #endregion

        #region Methods
        public async Task<SendEmailResult> SendEmail(string emailRecipient, string emailSubject, string emailText)
        {
            var result = new SendEmailResult();

            var restClient = new RestClient();
            var restRequest = new RestRequest(SendEmailEndpoint, Method.Post);

            var accessToken = await GetAccessToken(_sendEmailAccountEmail);
            if (!string.IsNullOrEmpty(accessToken))
            {
                restRequest.AddHeader("Authorization", $"Bearer {accessToken}");
                restRequest.AddHeader("Content-Type", "message/rfc822");

                var emailBody = new StringBuilder();
                emailBody.AppendLine($"To: {emailRecipient}");
                emailBody.AppendLine($"Subject: {emailSubject}");
                emailBody.AppendLine();
                emailBody.AppendLine($"{emailText}");

                restRequest.AddParameter("message/rfc822", emailBody.ToString(), ParameterType.RequestBody);

                var response = await restClient.ExecuteAsync(restRequest);
                if (!response.IsSuccessful)
                {
                    result.IsError = true;
                    result.ErrorMessage = GetRestErrorMessage(
                        errorMessage: "Failed to create a new user.",
                        endpointUrl: CreateUsersEndpoint,
                        method: restRequest.Method,
                        statusCode: response.StatusCode,
                        requestBody: emailBody,
                        responseBody: response.Content
                    );
                }
            }
            else
            {
                result.IsError = true;
                result.ErrorMessage = $"Failed to obtain an access token while sending email via POST {SendEmailEndpoint}.";
            }

            return result;
        }
        #endregion

        #region Nested Classes
        public class SendEmailResult : ApiBase.ApiResultBase
        {
            #region Properties
            public string ErrorMessage { get; set; }
            #endregion
        }
        #endregion
    }

    public class GmailUserManagementService : GmailServiceBase
    {
        #region Fields
        readonly string _createUserAccountEmail;

        const string AuthBaseUrl = "https://oauth2.googleapis.com/token";
        const string CreateUsersEndpoint = "https://admin.googleapis.com/admin/directory/v1/users";
        const string SendEmailEndpoint = "https://gmail.googleapis.com/upload/gmail/v1/users/me/messages/send";
        #endregion

        #region Constructors
        public GmailUserManagementService(string serviceAccountPrivateKey, string serviceAccountPrivateKeyId, string serviceAccountClientEmail, string createUserAccountEmail)
            : base(serviceAccountPrivateKey, serviceAccountPrivateKeyId, serviceAccountClientEmail)
        {
            _createUserAccountEmail = createUserAccountEmail;
        }
        #endregion

        #region Methods
        public async Task<CreateNewUserResult> CreateNewUser(string userEmail, string userPassword, string userFirstName, string userLastName)
        {
            var result = new CreateNewUserResult();

            var restClient = new RestClient();
            var restRequest = new RestRequest(CreateUsersEndpoint, Method.Post);

            var accessToken = await GetAccessToken(_createUserAccountEmail);
            if (!string.IsNullOrEmpty(accessToken))
            {
                var requestBody = new CreateUserRequest
                {
                    UserEmail = userEmail,
                    UserPassword = userPassword,
                    UserName = new CreateUserRequest.Name
                    {
                        UserFirstName = userFirstName,
                        UserLastName = userLastName
                    }
                };

                restRequest.AddHeader("Authorization", $"Bearer {accessToken}");
                restRequest.AddBody(requestBody);

                var response = await restClient.ExecuteAsync(restRequest);
                if (!response.IsSuccessful)
                {
                    result.IsError = true;
                    result.ErrorMessage = GetRestErrorMessage(
                        errorMessage: "Failed to create a new user.",
                        endpointUrl: CreateUsersEndpoint,
                        method: restRequest.Method,
                        statusCode: response.StatusCode,
                        requestBody: requestBody,
                        responseBody: response.Content
                    );
                }
            }
            else
            {
                result.IsError = true;
                result.ErrorMessage = $"Failed to obtain an access token while creating a new user via POST {CreateUsersEndpoint}.";
            }

            return result;
        }
        #endregion

        #region Nested Classes
        class CreateUserRequest
        {
            #region Properties
            [JsonPropertyName("primaryEmail")]
            public string UserEmail { get; set; }
            [JsonPropertyName("password")]
            public string UserPassword { get; set; }
            [JsonPropertyName("name")]
            public Name UserName { get; set; }
            #endregion

            #region Nested Classes
            public class Name
            {
                #region Properties
                [JsonPropertyName("givenName")]
                public string UserFirstName { get; set; }
                [JsonPropertyName("familyName")]
                public string UserLastName { get; set; }
                #endregion
            }
            #endregion
        }

        public class CreateNewUserResult : ApiBase.ApiResultBase
        {
            #region Properties
            public string ErrorMessage { get; set; }
            #endregion
        }
        #endregion
    }
}