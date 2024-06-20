using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Admin;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/change-language")]
    public class ChangeLanguageController : ControllerBase<ChangeLanguageModel>
    {
        #region Action
        [HttpGet]
        [Route("{culture:length(2)}", Name = ControllerActionRouteNames.Admin.ChangeLanguageController.ChangeLanguage)]
        public IActionResult ChangeLanguage(string culture)
        {
            Model.ChangeLanguage(culture);
            return Redirect(Model.UrlPreviousPage);
        } 
        #endregion
    }
}