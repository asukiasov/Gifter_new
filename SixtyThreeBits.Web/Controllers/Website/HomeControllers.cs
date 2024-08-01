using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Website.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Website;

namespace SixtyThreeBits.Web.Controllers.Website
{
    public class HomeController : WebsiteControllerBase<HomeModel>
    {
        #region Actions
        [Route("", Name = ControllerActionRouteNames.Website.Home.Index)]
        [Route("{RouteValueLanguageCode}", Name = ControllerActionRouteNames.Website.Home.IndexCulture)]
        public IActionResult Index()
        {
            return View(ViewNames.Website.Home.IndexView);
        }         
        #endregion
    }
}