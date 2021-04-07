using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Models;
using SixtyThreeBits.Web.Reusables.Core;

namespace SixtyThreeBits.Web.Controllers
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

        [Route("error/404/")]
        public IActionResult Error()
        {
            return View(ViewNames.Shared.NotFound);
        }
    }
}