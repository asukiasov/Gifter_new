using System;

namespace SixtyThreeBits.Core.Abstractions.Web
{
    public interface ICookieAssistance
    {
        #region Methods
        T Get<T>(string Key);
        string GetString(string Key);
        void Set<T>(string Key, T Value, DateTime? ExpirationDate);
        void Remove(string Key);
        #endregion
    }
}
