using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Admin;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/culture/change")]
    public class CultureChangeController : ControllerBase<CultureChangeModel>
    {
        #region Action
        [HttpGet]
        [Route("{culture:length(2)}", Name = ControllerActionRouteNames.Admin.CultureChangeController.ChangeLanguage)]
        public IActionResult ChangeLanguage(string culture)
        {
            Model.Change(culture);
            return Redirect(Model.UrlPreviousPage);
        } 
        #endregion
    }
}