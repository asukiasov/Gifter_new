using System;

namespace SixtyThreeBits.Core.Abstractions
{
    public interface ICookieAssistance
    {
        #region Methods
        T Get<T>(string Key);
        void Set<T>(string Key, T Value, DateTime? ExpirationDate);
        void Remove(string Key);
        #endregion
    }
}
