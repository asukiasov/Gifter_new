using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Extensions;
using SixtyThreeBits.Libraries;
using System;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Services
{
    public class TinifyService
    {
        #region Properties
        private string ApiKey;
        const string BaseUrl = "https://api.tinify.com";
        #endregion

        #region Constructors
        public TinifyService(string ApiKey)
        {
            this.ApiKey = ApiKey;
        }
        #endregion

        #region Methods
        public Task<IRestResponse> Execute(RestRequest Request)
        {
            var Client = new RestClient
            {
                BaseUrl = new Uri(BaseUrl),
                Authenticator = new HttpBasicAuthenticator("Basic", ApiKey)
            };
            Request.RequestFormat = DataFormat.Json;
            var ResponseTask = Client.ExecuteAsync(Request);
            return ResponseTask;
        }

        public async Task<ShrinkResult> ShrinkImage(string UrlFileToShrink)
        {
            var Result = new ShrinkResult();
            var Request = new RestRequest();
            Request.Method = Method.POST;
            Request.Resource = "shrink";
            Request.AddJsonBody(new
            {
                source = new
                {
                    url = UrlFileToShrink
                }
            });
            var Response = await Execute(Request);
            if(Response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                var Data = Response.Content.FromJsonTo<ShrinkResponse>();
                Result.UrlImageShrinked = Data.Output.Url;
                Result.FileBytes = DownloadFile(Result.UrlImageShrinked);
                Result.IsSucceess = true;
            }
            else
            {
                Result.IsError = true;
                Result.ErrorMessage = Response.Content;
            }

            return Result;
        }

        public byte[] DownloadFile(string FileDownloadUrl)
        {
            var Client = new RestClient(FileDownloadUrl);
            var Request = new RestRequest(Method.GET);
            var FileBytes = Client.DownloadData(Request);
            return FileBytes;
        }
        #endregion

        #region Sub Classes
        public class ShrinkResponse
        {
            #region Properties
            [JsonProperty("output")]
            public OutputResponse Output { get; set; }
            #endregion

            #region Sub Classes
            public class OutputResponse
            {
                #region Properties
                [JsonProperty("height")]
                public string Height { get; set; }
                [JsonProperty("ratio")]
                public string Ration { get; set; }
                [JsonProperty("size")]
                public string SizeBytes { get; set; }
                [JsonProperty("type")]
                public string ContentType { get; set; }
                [JsonProperty("url")]
                public string Url { get; set; }
                [JsonProperty("width")]
                public string Width { get; set; }
                #endregion
            } 
            #endregion
        }

        public class ResultBase
        {
            #region Properties
            public bool IsError { get; set; }
            public bool IsSucceess { get; set; }
            public string ErrorMessage { get; set; } 
            #endregion
        }
         
        public class ShrinkResult : ResultBase
        {
            #region Properties
            public string UrlImageShrinked { get; set; }
            public byte[] FileBytes { get; set; }
            #endregion
        }
        #endregion
    }
}
