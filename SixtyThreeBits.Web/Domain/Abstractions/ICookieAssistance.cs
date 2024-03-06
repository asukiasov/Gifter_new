using System;

namespace SixtyThreeBits.Core.Abstractions.Web
{
    public interface ICookieAssistance
    {
        #region Methods
        T Get<T>(string key);
        string GetString(string key);
        void Set<T>(string key, T value, DateTime? expirationDate);
        void Remove(string key);
        #endregion
    }
}
