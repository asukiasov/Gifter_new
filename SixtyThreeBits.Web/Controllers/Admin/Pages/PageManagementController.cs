using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Admin;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/page-management")]
    public class PageManagementController : AdminControllerBase<PageManagementModel>
    {
        #region Actions
        [Route("", Name = ControllerActionRouteNames.Admin.PagesManagementController.RedirectToChild)]
        public IActionResult RedirectToChild()
        {
            var redirectUrl = Model.GetRedirectUrl();
            if (string.IsNullOrWhiteSpace(redirectUrl))
            {
                return Model.GetNotFoundAdminViewResult();
            }
            else
            {
                return Redirect(redirectUrl);
            }
        }
        #endregion
    }    
}