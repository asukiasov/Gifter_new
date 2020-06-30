using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Web.Reusables.Core;

namespace SixtyThreeBits.Web.Admin.Models
{
    public class AdminLayoutViewModel : LayoutViewModelBase
    {
        #region Properties        
        public string UserFullname { get; set; }
        public ValueReference<bool> IsSidebarCollapsed { get; set; }
        public string UrlRelogin { get; set; }        
        #endregion
    }
}
