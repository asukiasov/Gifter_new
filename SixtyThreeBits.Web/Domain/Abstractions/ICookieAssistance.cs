using System;

namespace SixtyThreeBits.Core.Abstractions.Web
{
    public interface ICookieAssistance
    {
        #region Methods
        string Get(string key);        
        void Set(string key, string value, DateTime? expirationDate);
        void Remove(string key);
        #endregion
    }
}
