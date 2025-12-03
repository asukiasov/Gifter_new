using Newtonsoft.Json;
using RestSharp;
using SixtyThreeBits.Core.Infrastructure.Services.Base;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Services
{
    public class GoogleRecaptchaService : RestApiServiceBase
    {
        #region Properties
        const string _baseUrl = "https://www.google.com/recaptcha/api/siteverify";
        string _recaptchaSecretKey;
        decimal _recaptchaMinSuccessScore;
        #endregion

        #region Constructors
        public GoogleRecaptchaService(string recaptchaSecretKey, decimal recaptchaMinSuccessScore = 0.3m) : base(_baseUrl)
        {
            _recaptchaSecretKey = recaptchaSecretKey;
            _recaptchaMinSuccessScore = recaptchaMinSuccessScore;
        }
        #endregion

        #region Methods
        public async Task<VerifyRecaptchaResult> VerifyRecaptcha(string recaptchaClientResponseToken, string userIP = null)
        {
            var parameters = new List<Parameter>
            {
                new Parameter(Key: "secret", Value: _recaptchaSecretKey),
                new Parameter(Key: "response", Value: recaptchaClientResponseToken)
            };
            if (!string.IsNullOrWhiteSpace(userIP))
            {
                parameters.Add(new Parameter(Key: "remoteip", Value: userIP));
            }

            var executeResult = await ExecuteAsyncTask(
                resource: null,
                method: Method.Post,
                parameters: parameters
            );
            var result = new VerifyRecaptchaResult(executeResult);

            var responseData = executeResult.ResponseContent.DeserializeJsonTo<verifyRecaptchaResponse>();
            if (responseData != null)
            {
                result.Score = responseData.Score;
                result.IsSuccess = responseData.Score >= _recaptchaMinSuccessScore;
            }            

            return result;
        }
        #endregion

        #region Nested Classes
        class verifyRecaptchaResponse
        {
            #region Properties
            [JsonProperty("success")]
            public bool IsSuccess { get; set; }
            [JsonProperty("score")]
            public decimal? Score { get; set; }
            [JsonProperty("action")]
            public string Action { get; set; }
            #endregion
        }

        public class VerifyRecaptchaResult : ApiResultBase
        {
            #region Properties            
            public decimal? Score { get; set; }
            #endregion

            #region Constructors
            public VerifyRecaptchaResult(ApiResultBase apiResultBase) : base(apiResultBase)
            {

            }
            #endregion
        }
        #endregion
    }
}
