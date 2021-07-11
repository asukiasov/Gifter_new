using SixtyThreeBits.Core.Abstractions;
using SixtyThreeBits.Core.Utilities;

namespace SixtyThreeBits.Web.Reusables.Core
{
    public class PageTitle : IPageTitle
    {
        #region Properties        
        string _TitleHead = Constants.ProjectName;
        string _TitleValue = Constants.ProjectName;

        public string TitleHead => _TitleHead;
        public string Value => _TitleValue;
        #endregion

        #region Methods
        public void Set(string PageTitle)
        {
            if (!string.IsNullOrWhiteSpace(PageTitle))
            {
                _TitleHead = $"{PageTitle} | {Constants.ProjectName}";
                _TitleValue = PageTitle;
            }
        }

        public override string ToString()
        {
            return _TitleValue;
        }
        #endregion
    }
}
