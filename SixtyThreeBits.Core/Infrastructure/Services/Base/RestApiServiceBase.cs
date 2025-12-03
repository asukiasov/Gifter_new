using RestSharp;
using RestSharp.Authenticators;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace SixtyThreeBits.Core.Infrastructure.Services.Base
{
    public class RestApiServiceBase
    {
        #region Properties
        string _baseUrl;
        IAuthenticator _authenticator;
        #endregion

        #region Constructors
        public RestApiServiceBase(string baseUrl, IAuthenticator authenticator = null)
        {
            _baseUrl = baseUrl;
            _authenticator = authenticator;
        }
        #endregion

        #region Methods        
        public async Task<ApiResultBase> ExecuteAsyncTask(string resource, Method method, List<Parameter> headers = null, List<Parameter> parameters = null, string body = null, byte[] bodyBinary = null, List<File> files = null)
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest();

            if (_authenticator != null)
            {
                request.Authenticator = _authenticator;
            }

            request.Resource = string.IsNullOrWhiteSpace(resource) ? null : $"/{resource.Trim('/')}";
            request.Method = method;

            if (headers != null)
            {
                foreach (var item in headers)
                {
                    request.AddHeader(item.Key, item.Value ?? "");
                }
            }
            if (parameters != null)
            {
                foreach (var item in parameters)
                {
                    request.AddParameter(item.Key, item.Value ?? "");
                }
            }
            if (!string.IsNullOrWhiteSpace(body))
            {
                request.AddBody(body);
            }
            if (bodyBinary?.Length > 0)
            {
                request.AddBody(bodyBinary, "application/octet-stream");
            }
            if (files != null)
            {
                foreach (var item in files)
                {
                    request.AddFile("attachment", item.FileBytes, item.Filename, "application/octet-stream");
                }
            }

            var response = await client.ExecuteAsync(request);

            var result = new ApiResultBase();
            result.RequestUrl = $"{_baseUrl}{resource}";
            result.RequestMethod = request.Method.ToString().ToUpper();

            if (headers != null)
            {
                result.RequestHeaders = string.Join(",", headers.Select(item => $"{item.Key}: {item.Value}"));
            }

            if (parameters is null)
            {
                result.RequestBody = body;
            }
            else
            {
                result.RequestBody = string.Join("&", parameters.Select(item => $"{item.Key}={item.Value}"));
            }



            result.ResponseStatusCode = (int)response.StatusCode;
            result.ResponseHeaders = response.Headers?.GroupBy(h => h.Name).ToDictionary(g => g.Key, g => g.First().Value?.ToString());
            result.ResponseRawBytes = response.RawBytes;
            result.ResponseContent = response.Content;

            var isStatusCodeSuccess = result.ResponseStatusCode >= 200 && result.ResponseStatusCode < 300;
            if (isStatusCodeSuccess)
            {
                result.IsSuccess = true;
            }
            else if (response.ResponseStatus == ResponseStatus.TimedOut)
            {
                result.IsTimeout = true;
                result.ResponseContent = response.ErrorException?.Message;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(result.ResponseContent))
                {
                    result.ResponseContent = response.ErrorException?.Message;
                }
            }

            return result;
        }

        public void SetBaseUrl(string BaseUrl)
        {
            _baseUrl = BaseUrl;
        }
        #endregion

        #region Nested Classes
        public class ApiResultBase
        {
            #region Properties
            public bool IsSuccess { get; set; }
            public bool IsTimeout { get; set; }
            public string RequestUrl { get; set; }
            public string RequestMethod { get; set; }
            public string RequestHeaders { get; set; }
            public string RequestBody { get; set; }
            public int? ResponseStatusCode { get; set; }
            public Dictionary<string, string> ResponseHeaders { get; set; }
            public byte[] ResponseRawBytes { get; set; }
            public string ResponseContent { get; set; }
            #endregion

            #region Constructors
            public ApiResultBase() { }
            public ApiResultBase(ApiResultBase resultBase)
            {
                RequestUrl = resultBase.RequestUrl;
                RequestMethod = resultBase.RequestMethod;
                RequestHeaders = resultBase.RequestHeaders;
                RequestBody = resultBase.RequestBody;
                ResponseStatusCode = resultBase.ResponseStatusCode;
                ResponseContent = resultBase.ResponseContent;
                IsSuccess = resultBase.IsSuccess;
            }
            #endregion
        }

        public record Parameter(string Key, string Value)
        {
        }

        public record File(string Filename, byte[] FileBytes)
        {
        }
        #endregion
    }
}
