using Microsoft.AspNetCore.Http;
using SixtyThreeBits.Core.Abstractions.Web;
using SixtyThreeBits.Libraries.Extensions;
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
        public T Get<T>(string key)
        {
            var result = default(T);
            if (_request.Cookies.ContainsKey(key))
            {
                result = _request.Cookies[key].DeserializeJsonTo<T>();
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

        public void Set<T>(string key, T value, DateTime? expirationDate = null)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                var options = new CookieOptions();
                options.Expires = expirationDate ?? DateTime.Now.AddDays(1);
                if (typeof(T) == typeof(string) || typeof(T).IsValueType)
                {
                    _response.Cookies.Append(key, value.ToString());
                }
                else
                {
                    _response.Cookies.Append(key, value.ToJson());
                }
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
