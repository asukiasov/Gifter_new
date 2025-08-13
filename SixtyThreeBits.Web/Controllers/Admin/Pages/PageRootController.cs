using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Admin;
using System;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Obsolete("NEED TO TAKE CARE FOR ROUTING")]
    [Route("admin/pages-management")]
    public class PageRootController : AdminControllerBase<PagesManagementModule>
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