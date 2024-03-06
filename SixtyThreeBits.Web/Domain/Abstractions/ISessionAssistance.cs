namespace SixtyThreeBits.Core.Abstractions.Web
{
    public interface ISessionAssistance
    {
        #region Methods
        void Clear();
        T Get<T>(string key);
        string GetSessionID();
        bool HasKey(string key);
        void Set<T>(string key, T value);
        void Remove(string key);
        #endregion
    }
}