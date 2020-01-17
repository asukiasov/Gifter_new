using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Web.Reusables.Core;

namespace SixtyThreeBits.Web.Admin.Models
{
    public class AdminLayoutViewModel : LayoutViewModelBase
    {
        #region Properties        
        public string UserFullname { get; set; }        
        public bool IsSidebarCollapsed { get; set; }
        public string UrlRelogin { get; set; }
        public string TextError { get; set; } = Resources.TextError;
        public string TextSuccess { get; set; } = Resources.TextSuccess;
        #endregion
    }
}
