using RestSharp;
using SixtyThreeBits.Libraries;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace SixtyThreeBits.Core.Services
{
    public class ApiBase
    {
        #region Properties
        string BaseUrl;
        #endregion

        #region Constructors
        public ApiBase(string BaseUrl)
        {
            this.BaseUrl = BaseUrl;
        }
        #endregion

        #region Methods        
        public async Task<ApiResultBase> ExecuteAsyncTask(string Resource, Method Method, List<SimpleKeyValue<string, string>> Headers, List<SimpleKeyValue<string, string>> Parameters, string Body)
        {
            var Client = new RestClient(BaseUrl);
            var Request = new RestRequest();

            Request.Resource = Resource;
            Request.Method = Method;

            if (Headers != null)
            {
                foreach (var Item in Headers)
                {
                    Request.AddHeader(Item.Key, Item.Value ?? "");
                }
            }
            if (Parameters != null)
            {
                foreach (var Item in Parameters)
                {
                    Request.AddParameter(Item.Key, Item.Value ?? "");
                }
            }
            if (!string.IsNullOrWhiteSpace(Body))
            {
                Request.AddBody(Body);
            }

            var Response = await Client.ExecuteAsync(Request);

            var Result = new ApiResultBase();
            Result.RequestUrl = $"{BaseUrl}{Resource}";
            Result.RequestHeaders = string.Join(",", Headers?.Select(Item => $"{Item.Key}: {Item.Value}"));
            Result.RequestMethod = Request.Method.ToString().ToUpper();
            if (Parameters is null)
            {
                Result.RequestBody = Body;
            }
            else
            {
                Result.RequestBody = string.Join("&", Parameters.Select(Item => $"{Item.Key}={Item.Value}"));
            }

            Result.ResponseStatusCode = (int)Response.StatusCode;
            Result.ResponseContent = Response.Content;

            return Result;
        }

        public void SetBaseUrl(string BaseUrl)
        {
            this.BaseUrl = BaseUrl;
        }
        #endregion

        #region Nested Classes
        public class ApiResultBase
        {
            #region Constructors
            public ApiResultBase() { }
            public ApiResultBase(ApiResultBase ResultBaseItem)
            {
                RequestUrl = ResultBaseItem.RequestUrl;
                RequestMethod = ResultBaseItem.RequestMethod;
                RequestHeaders = ResultBaseItem.RequestHeaders;
                RequestBody = ResultBaseItem.RequestBody;
                ResponseStatusCode = ResultBaseItem.ResponseStatusCode;
                ResponseContent = ResultBaseItem.ResponseContent;

            }
            #endregion

            #region Properties
            public bool IsSuccess { get; set; }
            public string RequestUrl { get; set; }
            public string RequestMethod { get; set; }
            public string RequestHeaders { get; set; }
            public string RequestBody { get; set; }
            public int? ResponseStatusCode { get; set; }
            public string ResponseContent { get; set; }
            #endregion
        } 
        #endregion

    }
}
