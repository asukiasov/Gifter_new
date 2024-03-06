namespace SixtyThreeBits.Core.Abstractions.Web
{
    public interface IPageTitle
    {
        #region Properties                
        public string TitleHead { get; }
        public string Value { get; }
        #endregion

        #region Methods
        public void Set(string pageTitle);
        public string ToString();
        #endregion
    }
}
