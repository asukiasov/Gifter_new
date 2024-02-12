using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Domain;

namespace SixtyThreeBits.Web.Admin.Controllers
{
    [Route("admin/change-language")]
    public class ChangeLanguageController : WebProjectControllerBase<ChangeLanguageModel>
    {
        #region Constructors
        public ChangeLanguageController()
        {
            Model = new ChangeLanguageModel();
        }
        #endregion

        [HttpGet]
        [Route("{culture:length(2)}", Name = ControllerActionRouteNames.Admin.ChangeLanguage.Page)]
        
        public IActionResult ChangeLanguage(string culture)
        {
            Model.ChangeLanguage(culture);
            return Redirect(Model.UrlPreviousPage);
        }
    }
}