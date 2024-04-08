using SixtyThreeBits.Web.Models.Base;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class HomeModel : ModelBase
    {
        public PageViewModel GetPageViewModel()
        {
            var viewModel = new PageViewModel();
            viewModel.UserFullname = User.UserFullname;
            viewModel.RoleName = User.RoleName;
            return viewModel;
        }

        #region Nested Classes
        public class PageViewModel
        {
            #region properties
            public string UserFullname { get; set; }
            public string RoleName { get; set; }
            #endregion
        }
        #endregion
    }
}
