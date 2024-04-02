using Microsoft.AspNetCore.Http;
using SixtyThreeBits.Core.Abstractions.Web;
using System;

namespace SixtyThreeBits.Web.Domain.Libraries
{
    public class CookieAssistance : ICookieAssistance
    {
        #region Properties
        readonly HttpRequest _request;
        readonly HttpResponse _response;
        #endregion

        #region Constructors
        public CookieAssistance(HttpRequest request, HttpResponse response)
        {
            _request = request;
            _response = response;
        }
        #endregion

        #region Methods        
        public string Get(string key)
        {
            var result = default(string);
            if (_request.Cookies.ContainsKey(key))
            {
                result = _request.Cookies[key];
            }

            return result;
        }

        public string GetString(string key)
        {
            var Result = default(string);
            if (_request.Cookies.ContainsKey(key))
            {
                Result = _request.Cookies[key].ToString();
            }

            return Result;
        }

        public void Set(string key, string value, DateTime? expirationDate = null)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                var options = new CookieOptions();
                options.Expires = expirationDate ?? DateTime.Now.AddDays(1);
                _response.Cookies.Append(key, value, options);
            }
        }

        public void Remove(string key)
        {
            if (_request.Cookies.ContainsKey(key))
            {
                _response.Cookies.Delete(key);
            }
        }
        #endregion
    }
}
