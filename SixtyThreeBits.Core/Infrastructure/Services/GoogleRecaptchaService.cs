using Newtonsoft.Json;
using RestSharp;
using SixtyThreeBits.Libraries.Extensions;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Services
{
    public class GoogleRecaptchaService
    {
        #region Properties
        const string baseUrl = "https://www.google.com/recaptcha/api/siteverify";
        string _recaptchaSecretKey;
        decimal _recaptchaMinSuccessScore;
        #endregion

        #region Constructors
        public GoogleRecaptchaService(string recaptchaSecretKey, decimal recaptchaMinSuccessScore = 0.3m)
        {
            _recaptchaSecretKey = recaptchaSecretKey;
            _recaptchaMinSuccessScore = recaptchaMinSuccessScore;
        }
        #endregion

        #region Methods
        public async Task<VerifyRecaptchaResult> VerifyRecaptcha(string recaptchaClientResponseToken, string userIP = null)
        {
            var result = new VerifyRecaptchaResult();

            var request = new RestRequest();
            request.Method = Method.Post;
            request.RequestFormat = DataFormat.Json;
            request.AddParameter("secret", _recaptchaSecretKey);
            request.AddParameter("response", recaptchaClientResponseToken);
            if (!string.IsNullOrWhiteSpace(userIP))
            {
                request.AddParameter("remoteip", userIP);
            }

            var client = new RestClient(baseUrl);
            var response = await client.ExecuteAsync(request);
            var responseData = response.Content.DeserializeJsonTo<verifyRecaptchaResponse>();

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

        public class VerifyRecaptchaResult
        {
            #region Properties
            public bool IsSuccess { get; set; }
            public decimal? Score { get; set; }
            #endregion
        }
        #endregion
    }
}
