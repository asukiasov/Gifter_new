using SixtyThreeBits.Core.Abstractions.Web;

namespace SixtyThreeBits.Web.Domain.Libraries
{
    public class PageTitle : IPageTitle
    {
        #region Properties        
        readonly string _projectName;

        public string TitleHead { get; private set; }
        public string Value { get; private set; }
        #endregion

        #region Constructors
        public PageTitle(string projectName)
        {
            _projectName = projectName;
            TitleHead = projectName;
            Value = projectName;
        }
        #endregion

        #region Methods
        public void Set(string pageTitle)
        {
            if (!string.IsNullOrWhiteSpace(pageTitle))
            {
                TitleHead = $"{pageTitle} | {_projectName}";
                Value = pageTitle;
            }
        }

        public override string ToString()
        {
            return Value;
        }
        #endregion
    }
}