using Microsoft.AspNetCore.Http;
using SixtyThreeBits.Core.Abstractions;
using SixtyThreeBits.Libraries;
using System;

namespace SixtyThreeBits.Web.Reusables
{
    public class CookieAssistance : ICookieAssistance
    {
        #region Properties
        HttpRequest Request;
        HttpResponse Response;
        #endregion

        #region Constructors
        public CookieAssistance(HttpRequest Request, HttpResponse Response)
        {
            this.Request = Request;
            this.Response = Response;
        }
        #endregion

        #region Methods        
        public T Get<T>(string Key)
        {
            if (Request.Cookies.ContainsKey(Key))
            {
                return Request.Cookies[Key].DeserializeJsonTo<T>();
            }
            else
            {
                return default(T);
            }
        }

        public void Set<T>(string Key, T Value, DateTime? ExpirationDate = null)
        {
            if (!string.IsNullOrWhiteSpace(Key))
            {
                var options = new CookieOptions();
                options.Expires = ExpirationDate.HasValue ? ExpirationDate.Value : DateTime.Now.AddDays(1);
                Response.Cookies.Append(Key, Value.ToJSON());
            }
        }         

        public void Remove(string Key)
        {
            if (Request.Cookies.ContainsKey(Key))
            {
                Response.Cookies.Delete(Key);
            }
        }
        #endregion
    }
}
