using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Base;

namespace SixtyThreeBits.Web.Domain.ViewModels.Admin
{
    public class UserLayoutViewModel : LayoutViewModelBase
    {
        #region Properties        
        public string UserFullname { get; set; }
        public string UserDateCreated { get; set; }
        public bool UserIsActive { get; set; }
        public string RoleName { get; set; }

        public readonly string HeaderSectionName = WebConstants.ViewSections.HeaderSection;
        public readonly string FooterSectionName = WebConstants.ViewSections.FooterSection;
        #endregion
    }
}