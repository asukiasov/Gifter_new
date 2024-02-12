namespace SixtyThreeBits.Core.Abstractions.Web
{
    public interface ISessionAssistance
    {
        #region Methods
        void Clear();
        T Get<T>(string Key);
        string GetSessionID();
        bool HasKey(string Key);
        void Set<T>(string Key, T Value);
        void Remove(string Key);
        #endregion
    }
}
