using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Website.Base;
using SixtyThreeBits.Web.Domain;
using SixtyThreeBits.Web.Models.Website;

namespace SixtyThreeBits.Web.Controllers.Website
{
    public class HomeController : WebsiteControllerBase<HomeModel>
    {
        public HomeController()
        {
            Model = new HomeModel();
        }

        [Route("", Name = ControllerActionRouteNames.Website.Home.Index)]
        [Route("{Culture:length(2)}", Name = ControllerActionRouteNames.Website.Home.IndexCulture)]
        public IActionResult Index()
        {
            return View(ViewNames.Website.Home.Page);
        }        
    }
}